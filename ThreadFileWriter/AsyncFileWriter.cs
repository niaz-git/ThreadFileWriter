using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ThreadFileWriter
{
    public class AsyncFileWriter : IFileWriter, IAsyncDisposable
    {
        private readonly string _filePath;
        private readonly StreamWriter _writer;
        private readonly Channel<LogEntry> _channel;
        private readonly Task _processingTask;

        private int _lineCounter = 0;
        private bool _disposed;

        public AsyncFileWriter(string filePath)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));

            var fileStream = new FileStream(
                _filePath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                4096,
                useAsync: true);

            _writer = new StreamWriter(fileStream, Encoding.UTF8);

            _channel = Channel.CreateUnbounded<LogEntry>(
                new UnboundedChannelOptions
                {
                    SingleReader = true,
                    SingleWriter = false
                });

            _processingTask = ProcessQueueAsync();
        }

        public async Task AppendFileContent(int threadId)
        {
            ThrowIfDisposed();

            int newLine = Interlocked.Increment(ref _lineCounter);

            try
            {
                await _channel.Writer.WriteAsync(
                    new LogEntry(newLine, threadId));
            }
            catch (ChannelClosedException)
            {
                throw new InvalidOperationException(
                    "Cannot write to file. The writer has already been completed.");
            }
        }

        private async Task ProcessQueueAsync()
        {
            try
            {
                await foreach (var entry in _channel.Reader.ReadAllAsync())
                {
                    await WriteLineAsync(entry.LineNumber, entry.ThreadId);
                }
            }
            catch (Exception ex)
            {
                // Ensure the channel is faulted so writers stop
                _channel.Writer.TryComplete(ex);
                throw;
            }
        }

        private async Task WriteLineAsync(int lineNumber, int threadId)
        {
            string timestamp = DateTime.UtcNow.ToString("HH:mm:ss.fff");
            string line = $"{lineNumber}, {threadId}, {timestamp}";

            await _writer.WriteLineAsync(line);
        }

        public async Task InitializeFile()
        {
            ThrowIfDisposed();
            await WriteLineAsync(0, 0);
        }

        public async Task Complete()
        {
            if (_disposed) return;

            _channel.Writer.TryComplete();

            try
            {
                await _processingTask.ConfigureAwait(false);
            }
            catch
            {
                // Let caller observe background exception
                throw;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;
            _disposed = true;

            _channel.Writer.TryComplete();

            try
            {
                await _processingTask.ConfigureAwait(false);
            }
            finally
            {
                await _writer.DisposeAsync();
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(AsyncFileWriter));
        }

        private record LogEntry(int LineNumber, int ThreadId);
    }
}