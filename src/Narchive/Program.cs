using McMaster.Extensions.CommandLineUtils;

namespace Narchive
{
    [Command(Name = "Narchive",
        Description = "Creates and extracts NARC archives.",
        ThrowOnUnexpectedArgument = false)]
    [Subcommand(typeof(CreateCommand))]
    [Subcommand(typeof(ExtractCommand))]
    [HelpOption("-? | -h | --help",
        Description = "Show help information.")]
    class Program
    {
        static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

#pragma warning disable IDE0051 // This method is used. Disable the message that says otherwise.
        private void OnExecute(CommandLineApplication app) => app.ShowHelp();
#pragma warning restore IDE0051
    }
}
