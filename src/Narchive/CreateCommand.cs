using McMaster.Extensions.CommandLineUtils;
using Narchive.Formats;
using System;

namespace Narchive
{
    [Command("create",
        Description = "Creates a NARC archive.")]
    [HelpOption("-? | -h | --help",
        Description = "Show help information.")]
    class CreateCommand
    {
        [Argument(0, "output", "The output name of the NARC archive to create.")]
        public string OutputPath { get; set; }

        [Argument(1, "input", "The folder containing the files and folders to add to the NARC archive.")]
        public string InputPath { get; set; }

        [Option("--nofilenames", "Specifies the entries in the NARC archive will not have filenames.", CommandOptionType.NoValue)]
        public bool NoFilenames { get; set; }

        private void OnExecute(IConsole console)
        {
            var reporter = new ConsoleReporter(console);

            try
            {
                NarcArchive.Create(InputPath, OutputPath, !NoFilenames);
            }
            catch (Exception e)
            {
                reporter.Error(e.Message);
            }
        }
    }
}
