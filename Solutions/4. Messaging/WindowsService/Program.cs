﻿using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;
using Topshelf;

namespace WindowsService
{
    class Program
    {
        private const string InputFolderName = "in";
        private const string OutputFolderName = "out";

        static void Main(string[] args)
        {
            var currentDirectory = Directory.GetCurrentDirectory();

            var inputDirectoryPath = Path.Combine(currentDirectory, InputFolderName);
            var outputDirectoryPath = Path.Combine(currentDirectory, OutputFolderName);

            Console.WriteLine(currentDirectory);

            Console.WriteLine(
                $"Put images or Pdf documents into {inputDirectoryPath}.{Environment.NewLine}Result you can see in {outputDirectoryPath}");

            var logConfig = new LoggingConfiguration();
            var target = new FileTarget
            {
                Name = "Def",
                FileName = Path.Combine(currentDirectory, "log.txt"),
                Layout = "${date} ${message} ${onexception:inner=${exception:format=toString}}"
            };

            logConfig.AddTarget(target);
            logConfig.AddRuleForAllLevels(target);

            var logFactory = new LogFactory(logConfig);

            HostFactory.Run(
                conf => conf.Service<IFileMonitoringService>(
                    serv =>
                    {
                        serv.ConstructUsing(() => GetFileMonitoringService(inputDirectoryPath, outputDirectoryPath));
                        serv.WhenStarted(s => s.Start());
                        serv.WhenStopped(s => s.Stop());
                    }
                ).UseNLog(logFactory)
            );
        }

        private static IFileConcatenationService GetFileConcatenationService(string outputDirectoryPath) =>
            new FileConcatenationService(outputDirectoryPath, GetFileAvailabilityService());

        private static IFileAvailabilityService GetFileAvailabilityService()
            => new FileAvailabilityService();

        private static IFileMonitoringService GetFileMonitoringService(string inputDirectoryName, string outputDirectoryPath)
            => new FileMonitoringService(GetFileConcatenationService(outputDirectoryPath), inputDirectoryName);
    }
}
