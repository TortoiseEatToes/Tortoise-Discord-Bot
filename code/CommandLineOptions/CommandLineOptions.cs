using CommandLine;

namespace TortoiseDiscordBot.code.CommandLineOptions
{
    internal class CommandLineOptions
    {

        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        [Option('t', Required = false, HelpText = "Bot Token.")]
        public string BotToken { get; set; }

        [Option('s', Required = false, HelpText = "Bot Secret.")]
        public string BotSecret { get; set; }
    }
}
