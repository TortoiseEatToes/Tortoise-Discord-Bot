using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TortoiseBotWPF.CommandLine;

namespace TortoiseBotWPF
{
    internal class CommandLineVarString : CommandLineVar
    {
        private string _argName;
        private string _value;
        private bool _hasBeenSet;

        public CommandLineVarString(string argName)
        {
            _argName = argName;
            _value = "";
            CommandLineVarManager.RegisterCommandLineVar(this);
        }

        public CommandLineVarString(string argName, string defaultValue)
        {
            _argName = argName;
            _value = defaultValue;
            CommandLineVarManager.RegisterCommandLineVar(this);
        }

        public override void Parse(string argValue)
        {
            _value = argValue;
            _hasBeenSet = true;
        }

        public override string GetArgName()
        {
            return _argName;
        }

        public override string GetValue()
        {
            return _value;
        }

        public override bool HasBeenSet()
        {
            return _hasBeenSet;
        }
    }
}
