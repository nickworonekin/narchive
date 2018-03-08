﻿using McMaster.Extensions.CommandLineUtils;
using Narchive.Formats;
using System;

namespace Narchive
{
    [Command("extract",
        Description = "Extracts a NARC archive.")]
    [HelpOption("-? | -h | --help",
        Description = "Show help information.")]
    class ExtractCommand
    {
        [Argument(0, "input", "The NARC archive to extract.")]
        public string InputPath { get; set; }

        [Option("-o | --output", "The folder to extract the NARC archive to.", CommandOptionType.SingleValue)]
        public string OutputPath { get; set; }

        private void OnExecute(IConsole console)
        {
            var reporter = new ConsoleReporter(console);

            try
            {
                if (OutputPath == null)
                {
                    OutputPath = Environment.CurrentDirectory;
                }

                NarcArchive.Extract(InputPath, OutputPath);
            }
            catch (Exception e)
            {
                reporter.Error(e.Message);
            }
        }
    }
}