using McMaster.Extensions.CommandLineUtils;
using Narchive.Formats;
using Narchive.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Narchive
{
    [Command("create",
        Description = "Creates a NARC archive.")]
    [HelpOption("-? | -h | --help",
        Description = "Show help information.")]
    class CreateCommand
    {
        [Required]
        [Argument(0, "output", "The output name of the NARC archive to create.")]
        public string OutputPath { get; set; }

        [Required]
        [Argument(1, "input", "The folder containing the files and folders to add to the NARC archive.")]
        public string InputPath { get; set; }

        [Option("--nofilenames", "Specifies the entries in the NARC archive will not have filenames.", CommandOptionType.NoValue)]
        public bool NoFilenames { get; set; }

        private void OnExecute(IConsole console)
        {
            var reporter = new ConsoleReporter(console);

            try
            {
                var rootDirectory = NarcArchiveRootDirectoryEntry.CreateFromPath(InputPath);
                NarcArchive.Create(rootDirectory, OutputPath, !NoFilenames);
            }
            catch (Exception e)
            {
                reporter.Error(e.Message);
            }
        }
    }
}
