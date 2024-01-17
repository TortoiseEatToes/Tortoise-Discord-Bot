using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TortoiseBotWPF;
using TortoiseBotWPF.CommandLine;

namespace TortoiseDiscordBot.code.CommandLineOptions
{
    internal class CommandLineOptionsManager
    {
        private static CommandLineOptionsManager? commandLineOptionsManager;
        private CommandLineOptions commandLineOptions = new CommandLineOptions();

        public static CommandLineOptions GetOptions()
        {
            if (commandLineOptionsManager is not null)
            {
                return commandLineOptionsManager.commandLineOptions;
            }
            return new CommandLineOptions();
        }

        public static void Initialize()
        {
            commandLineOptionsManager = new CommandLineOptionsManager();
            commandLineOptionsManager.privInitialize();
        }

        private void privInitialize()
        {
            ParserResult<CommandLineOptions> parserResult = Parser.Default.ParseArguments<CommandLineOptions>(Environment.GetCommandLineArgs());
            commandLineOptions = parserResult.Value;
        }
    }
}
