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

        public Task InitializeFile()
        {
            throw new NotImplementedException();
        }
    }
}
