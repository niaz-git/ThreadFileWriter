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
        }
    }
}
