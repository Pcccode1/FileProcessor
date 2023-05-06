using FileProcessor.Models;
using FileProcessor.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace FileProcessor.Control
{
    public class FileParser
    {

        private ILogger logger { get; set; }
        private IStorageService storageservice { get; set; }
        private Settings settings { get; set; }

        public FileParser(ILogger<FileParser> logger, IStorageService storageservice, Settings settings)
        {
            this.logger = logger;
            this.storageservice = storageservice;
            this.settings = settings;
        }


        public Party Parse(string path)
        {
            logger.LogInformation("Validation started");

            if (Path.GetExtension(path).ToUpper() != ".ZIP")
            {
                throw new Exception($"This file is not a Zip file {path}");
            }
            logger.LogInformation("It is a ZIP file");
            // Convert zip file into memeory stream
            var stream = storageservice.GetFile(path);

            //To query the files inside of a zip convert this to ZipArchive
            ZipArchive zipFile = null;
            //The ZIP file is not corrupt, that is the files inside can be queried
            try
            {
                 zipFile = new ZipArchive(stream);
                 logger.LogInformation("Zip file is not corrupted");
            }
            catch (Exception ex)
            {
                throw new Exception($"The ZIP file is corrupted - {path}",ex);
            }
            
            //Pass ziparchive for validation 
            
            if (ValidateFilesInZip(zipFile))
            {
                return ValidatePartyXml(zipFile);
            }
            logger.LogInformation("Validations have been completed");
            return null;
        }

        public bool ValidateFilesInZip(ZipArchive zipFile)
        {
            logger.LogInformation("Validating the ZIP file contains only XLS|X, DOC|X, PDF, MSG and image files");
            string doctypes  = settings.DocumentTypes;
            var extensionList = new List<string>();
            var libraries = zipFile.Entries.Where(d => !(doctypes.ToUpper().Split(",").ToList().Contains(Path.GetExtension(d.Name.ToUpper()))))
                         .Select(d => d.Name);
            if (libraries.Any())
            {
                throw new Exception("Some unwanted files are there in the zip file");
            }
            logger.LogInformation("The ZIP file contains only XLS|X, DOC|X, PDF, MSG and image files ");
            return true;
        }

        public Party ValidatePartyXml(ZipArchive zipFile)
        {
            logger.LogInformation("Validating Party.XML file");
            var path = new Uri(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)).LocalPath;
            XmlSchemaSet schema = new XmlSchemaSet();
            string applicationNo = string.Empty;
            schema.Add("", path + "\\XSD\\party.xsd");
            Party party = null;
            try
            {

                foreach (var entry in zipFile.Entries)
                {
                    if (entry.Name.EndsWith(".XML"))
                    {
                        using (var stream = entry.Open())
                        {
                            XmlReader rd = XmlReader.Create(stream);
                            XDocument doc = XDocument.Load(rd);
                            doc.Validate(schema, null);

                        }

                        using (var stream = entry.Open())
                        {
                            XmlReader rd = XmlReader.Create(stream);
                            XmlSerializer ser = new XmlSerializer(typeof(Party));

                            party = (Party)ser.Deserialize(rd);

                        }
                    }
                }

                logger.LogInformation("The party.xml Is valid");

            }
            catch(XmlSchemaValidationException ex)
            {
                throw new Exception("The party.xml file failed in schema validation", ex);
            }

            return party;
        }

    }
}
