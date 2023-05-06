using System;
using System.IO.Compression;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using FileProcessor.Control;
using FileProcessor.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using FileProcessor.Models;
using Serilog;

namespace FileProcessor
{
    public static class Program
    {



        static void Main(string[] args)
        {
            //DI
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var fileProcess = serviceProvider.GetService<FileProcess>();


            //File Processing 
            fileProcess.ProcessFiles();
            Console.ReadLine();
        }
        private static void ConfigureServices(IServiceCollection services)
        {
            //Configuration of Settings
            IConfiguration Configuration = new ConfigurationBuilder()
           .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
           .Build();

            //Binding SettingsModel to Configuration
            var settings = new Settings();
            Configuration.Bind("Settings", settings);


            //Binding EmailModel to Configuration
            var emailConfiguration = new EmailConfiguration();
            Configuration.Bind("EmailConfiguration", emailConfiguration);

            //CreateLog
            Log.Logger = new LoggerConfiguration()
           .WriteTo.File("consoleapp.log")
           .WriteTo.Console()
           .CreateLogger();


            //DI Registration 

            services.AddLogging(configure => configure.AddConsole())
                .AddLogging(configure => configure.AddSerilog())
               .AddTransient<FileParser>()
               .AddTransient<FileProcess>()
               .AddSingleton(settings)
               .AddSingleton(emailConfiguration)
               .AddTransient<INotificationService, EmailNotificationService>()
               .AddTransient<IStorageService, FileSystemStorageService>();



        }

    }
}
