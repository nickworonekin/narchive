using McMaster.Extensions.CommandLineUtils;

namespace Narchive
{
    [Command(Name = "Narchive",
        Description = "Creates and extracts NARC archives.",
        ThrowOnUnexpectedArgument = false)]
    [Subcommand("create", typeof(CreateCommand))]
    [Subcommand("extract", typeof(ExtractCommand))]
    [HelpOption("-? | -h | --help",
        Description = "Show help information.")]
    class Program
    {
        static void Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        private void OnExecute(CommandLineApplication app) => app.ShowHelp();
    }
}
