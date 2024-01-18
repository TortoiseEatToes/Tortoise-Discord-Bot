using CommandLine;
using System;
using Tortoise;

namespace TortoiseDiscordBot.code.CommandLineOptions
{
    internal class CommandLineOptionsManager
    {
        private static CommandLineOptions commandLineOptions = new CommandLineOptions();

        public static CommandLineOptions GetOptions()
        {
            return commandLineOptions;
        }

        public static void Initialize()
        {
            string[] commandLineArgs = Environment.GetCommandLineArgs();
            ParserResult<CommandLineOptions> parserResult = Parser.Default.ParseArguments<CommandLineOptions>(commandLineArgs);
            switch (parserResult.Tag)
            {
                case ParserResultType.Parsed:
                    commandLineOptions = parserResult.Value;
                    break;
                case ParserResultType.NotParsed:
                    Logger.WriteLine("Failed to parse commandline args");
                    foreach(Error error in parserResult.Errors)
                    {
                        Logger.WriteLine($"Parser error '{error.Tag}' with '{error.ToString()}'");
                    }
                    break;
                default:
                    Logger.WriteLine("Unknown ParserResult");
                    break;
            }
        }
    }
}