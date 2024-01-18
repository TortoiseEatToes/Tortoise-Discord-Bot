using CommandLine;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tortoise;
using TortoiseBotWPF;

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
                    break;
                default:
                    Logger.WriteLine("Unknown ParserResult");
                    break;
            }
        }
    }
}
