using CommandLine;

namespace TortoiseDiscordBot.code.CommandLineOptions
{
    internal class CommandLineOptions
    {

        [Option(Default = null)]
        public string? BotToken { get; set; }

        [Option(Default = null)]
        public string? BotSecret { get; set; }

        [Option(Default = null)]
        public string? SettingsFileOverride { get; set; }

        [Option(Default = null)]
        public string? WorkingDirectory { get; set; }
    }
}
