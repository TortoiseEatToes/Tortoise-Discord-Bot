using CommandLine;

namespace TortoiseDiscordBot.code.CommandLineOptions
{
    internal class CommandLineOptions
    {

        [Option("BotToken", Default = null, Required = false)]
        public string? BotToken { get; set; }

        [Option("BotSecret", Default = null, Required = false)]
        public string? BotSecret { get; set; }

        [Option("SettingsFileOverride", Default = null, Required = false)]
        public string? SettingsFileOverride { get; set; }

        [Option("WorkingDirectory", Default = null, Required = false)]
        public string? WorkingDirectory { get; set; }
    }
}
