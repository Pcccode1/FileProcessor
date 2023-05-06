using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;


namespace FileProcessor.Services
{
    public class FileSystemStorageService : IStorageService
    {
        public FileSystemStorageService()
        {


        }
        /// <summary>
        /// Service for Get Single file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public MemoryStream GetFile(string path)
        {
            MemoryStream ms = new MemoryStream();
            using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
                file.CopyTo(ms);
            return ms;
        }
        /// <summary>
        /// Get all files
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IEnumerable<FileInfo> GetFiles(string path)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            IEnumerable<FileInfo> files = directoryInfo.EnumerateFiles();
            return files;

        }

    }
}
