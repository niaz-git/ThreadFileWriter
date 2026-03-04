using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadFileWriter
{
    public class AsyncFileWriter : IFileWriter, IAsyncDisposable
    {
        // File path location
        private readonly string _filePath; 
           
        private readonly StreamWriter _writer;


        public AsyncFileWriter(string filePath)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            try
            {
                var fileStream = new FileStream(
                  _filePath,
                  FileMode.Create,
                  FileAccess.Write,
                  FileShare.None,
                  4096,
                  useAsync: true);
                _writer = new StreamWriter(fileStream, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to initialize file writer: {ex.Message}");
                throw;
            }

        }
        public async Task AppendFileContent(int threadId)
        {
          
        }
        /// <summary>
        /// Disposes the StreamWriter asynchronously to ensure all data is flushed to the file and resources are released properly.
        /// </summary>
        /// <returns></returns>
        public async ValueTask DisposeAsync()
        {
            await _writer.DisposeAsync();
        }

        /// <summary>
        /// Initializes the file by writing a first line
        /// </summary>
        /// <returns></returns>
        public async Task InitializeFile()
        {
          await   WriteLineAsync(0, 0);
        }


        /// <summary>
        /// Writes a line to the file with the format: "LineNumber, ThreadId, Timestamp"
        /// </summary>
        /// <param name="lineNumber"></param>
        /// <param name="threadId"></param>
        /// <returns></returns>

        private async Task  WriteLineAsync(int lineNumber, int threadId)
        {
            try
            {
                string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
                string line = $"{lineNumber}, {threadId}, {timestamp}";

                await _writer.WriteLineAsync(line);
                //await _writer.FlushAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write to file: {ex.Message}");
                throw;
            }
        }
    }
}
