using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadFileWriter
{
    public interface IFileWriter
    {
        Task InitializeFile();
        Task AppendFileContent(int threadId);
        Task Complete();

    }
}
