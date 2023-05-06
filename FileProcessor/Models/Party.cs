using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FileProcessor.Models
{
    [XmlRoot("party")]
    public class Party
    {
        [XmlElement("name")]
        public string Name { get; set; }
        [XmlElement("email")]

        public string Email { get; set; }
        [XmlElement("applicationno")]

        public int ApplicationNo { get; set; }
    }
}
