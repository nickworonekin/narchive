using McMaster.Extensions.CommandLineUtils;
using Narchive.Formats;
using System;
using System.ComponentModel.DataAnnotations;

namespace Narchive
{
    [Command("create",
        Description = "Creates a NARC archive.")]
    [HelpOption("-? | -h | --help",
        Description = "Show help information.")]
    class CreateCommand
    {
        [Required]
        [LegalFilePath]
        [Argument(0, "output", "The output name of the NARC archive to create.")]
        public string OutputPath { get; set; }

        [Required]
        [DirectoryExists]
        [Argument(1, "input", "The folder containing the files and folders to add to the NARC archive.")]
        public string InputPath { get; set; }

        [Option("-nf | --nofilenames", "Specifies the entries in the NARC archive will not have filenames.", CommandOptionType.NoValue)]
        public bool NoFilenames { get; set; }

#pragma warning disable IDE0051 // This method is used. Disable the message that says otherwise.
        private int OnExecute(IConsole console)
#pragma warning restore IDE0051
        {
            var reporter = new ConsoleReporter(console);

            try
            {
                var rootDirectory = new NarcArchiveRootDirectoryEntry(InputPath);
                NarcArchive.Create(rootDirectory, OutputPath, !NoFilenames);

                return 0;
            }
            catch (Exception e)
            {
                reporter.Error(e.Message);

                return e.HResult;
            }
        }
    }
}
