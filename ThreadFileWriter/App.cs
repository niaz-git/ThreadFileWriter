using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadFileWriter
{
    public class App
    {
        // Dependency injection of the file writer
        private readonly IFileWriter _fileWriter;
        public App(IFileWriter fileWriter)
        {
            _fileWriter = fileWriter;
        }

        //background task to initialize the file before starting the threads
        public async Task RunAsync()
        {
            await _fileWriter.InitializeFile();
            var tasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    int threadId = Environment.CurrentManagedThreadId;

                    for (int j = 0; j < 10; j++)
                    {
                        await _fileWriter.AppendFileContent(threadId);
                    }
                }));
            }
            await Task.WhenAll(tasks);
            await _fileWriter.Complete();
            Console.WriteLine("All threads completed.");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

        }
    }
}
