using System.IO;
using System.Collections.Generic;


namespace FileProcessor.Services
{
   public interface IStorageService
    {
       public IEnumerable<FileInfo> GetFiles(string Path);

        public MemoryStream GetFile(string Path);
    }
}
