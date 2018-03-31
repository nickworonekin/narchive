using McMaster.Extensions.CommandLineUtils;
using Narchive.Formats;
using System;
using System.ComponentModel.DataAnnotations;

namespace Narchive
{
    [Command("extract",
        Description = "Extracts a NARC archive.")]
    [HelpOption("-? | -h | --help",
        Description = "Show help information.")]
    class ExtractCommand
    {
        [Required]
        [FileExists]
        [Argument(0, "input", "The NARC archive to extract.")]
        public string InputPath { get; set; }

        [LegalFilePath]
        [Option("-o | --output", "The folder to extract the NARC archive to.", CommandOptionType.SingleValue)]
        public string OutputPath { get; set; }

        [Option("-nf | --nofilenames", "Ignores entry filenames and extracts using its index.", CommandOptionType.NoValue)]
        public bool IgnoreFilenames { get; set; }

        private int OnExecute(IConsole console)
        {
            var reporter = new ConsoleReporter(console);

            try
            {
                if (OutputPath == null)
                {
                    OutputPath = Environment.CurrentDirectory;
                }

                NarcArchive.Extract(InputPath, OutputPath, IgnoreFilenames);

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
