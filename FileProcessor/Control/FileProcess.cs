using FileProcessor.Models;
using FileProcessor.Services;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace FileProcessor.Control
{
    public class FileProcess
    {
        private ILogger logger { get; set; }
        private IStorageService storageservice { get; set; }
        private INotificationService notificationService { get; set; }
        public string SourceFolderPath { get; set; }
        public FileParser FileParser { get; set; }
        private Settings settings { get; set; }
        public FileProcess(ILogger<FileProcess> logger, IStorageService storageservice, INotificationService notificationService, FileParser fileParser, Settings settings)
        {
            this.logger = logger;
            this.storageservice = storageservice;
            this.notificationService = notificationService;
            this.settings = settings;
            SourceFolderPath = settings.SourceFolderName;
            FileParser = fileParser;

        }

        /// <summary>
        /// Processing files
        /// </summary>
        public void ProcessFiles()
        {
            //Collect all file informations from the source folder
            logger.LogInformation("Start reading files from source path");
            var files = storageservice.GetFiles(SourceFolderPath);

            logger.LogInformation("Collecting informations about all the files has been completed");
            logger.LogInformation($"Total number of files : { files.Count()}");

            foreach (var file in files)
            {
                logger.LogInformation($"Start File process FileName :{file.FullName}");

                try
                {
                    // Parse file for validation
                    Party party = FileParser.Parse(file.FullName);
                    //Completing all the validaitons and getting application no from the party.xml
                    if (party != null && party.ApplicationNo > 0)
                    {
                        //extracting the zip in to [applicationno]-[guid] 
                        ExtractZip(file.FullName, party.ApplicationNo);
                        // a notification sent by email to an administrator. 
                        logger.LogInformation($"Sending notification to administrator ");
                        notificationService.Send($"File Processed successfully - {file.Name} - {party.ApplicationNo}");

                    }
                    logger.LogInformation($"The file process has been completed FileName :{file.FullName}");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "exception happened while parsing");
                    logger.LogInformation($"The file is not processed FileName:{file.FullName}");
                    //move file some other folder??
                    notificationService.Send($"File Process failed {ex.Message}");

                }


            }

        }
        /// <summary>
        /// Extract Zip file
        /// </summary>
        /// <param name="zipFilePath"></param>
        /// <param name="applicationNo"></param>
        public void ExtractZip(string zipFilePath, int applicationNo)
        {
            logger.LogInformation("Extraction started");
            string extractPath = settings.ExtractFolderName;

            //ZipFile.CreateFromDirectory(startPath, zipPath);
            string extractFilename = applicationNo + "-" + Guid.NewGuid().ToString();
            string extractionPath = Path.Combine(extractPath, extractFilename);

            ZipFile.ExtractToDirectory(zipFilePath, extractionPath);
            logger.LogInformation($"Extraction completed path - {extractionPath}");
        }
    }
}
