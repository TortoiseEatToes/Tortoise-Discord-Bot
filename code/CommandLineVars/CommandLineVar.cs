using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TortoiseBotWPF.CommandLine
{
    internal abstract class CommandLineVar
    {
        public abstract void Parse(string argValue);

        public abstract string GetArgName();

        public abstract string GetValue();

        public abstract bool HasBeenSet();
    }
}
