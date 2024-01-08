using System;
using System.Collections.Generic;
using System.Linq;

namespace TortoiseBotWPF.CommandLine
{
    internal class CommandLineVarManager
    {
        private static CommandLineVarManager? commandLineVarManager;
        private Dictionary<string, CommandLineVar> commandLineVars = new Dictionary<string, CommandLineVar>();

        public static void ParseCommandLineArgs()
        {
            if (commandLineVarManager is not null)
            {
                commandLineVarManager.privParseCommandLineArgs();
            }
        }

        public static void RegisterCommandLineVar(CommandLineVar commandLineVar)
        {
            if (commandLineVarManager is null)
            {
                commandLineVarManager = new CommandLineVarManager();
            }
            commandLineVarManager.privRegisterCommandLineVar(commandLineVar);
        }

        private void privParseCommandLineArgs()
        {
            foreach (string commandLineArg in Environment.GetCommandLineArgs())
            {
                string[] commandLineArgSplit = commandLineArg.Split('=');
                if (commandLineArgSplit.Count() != 2)
                {
                    continue;
                }

                CommandLineVar? commandLineVar;
                if (commandLineVars.TryGetValue(commandLineArgSplit[0], out commandLineVar))
                {
                    commandLineVar.Parse(commandLineArgSplit[1]);
                }
            }
        }

        private void privRegisterCommandLineVar(CommandLineVar commandLineVar)
        {
            commandLineVars.Add(commandLineVar.GetArgName(), commandLineVar);
        }
    }
}
