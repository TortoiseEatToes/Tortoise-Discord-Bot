using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tortoise
{
    public abstract class TortoiseBotCommand
    {
        public abstract string GetDisplayName();
        public abstract string GetDescription();
        
    }
}
