using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tortoise
{
    internal abstract class TortoiseBotCommand
    {
        public abstract string GetDescription();

        public abstract string GetDisplayName();
    }
}
