using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessor.Models
{
    public sealed class Settings
    {
        public  string SourceFolderName { get; set; }
        public string ExtractFolderName { get; set; }
        public string DocumentTypes { get; set; }
        public string PartyFilename { get; set; }
        
    }
}
