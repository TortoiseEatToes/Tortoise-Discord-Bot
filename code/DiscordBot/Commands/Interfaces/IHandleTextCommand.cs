using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tortoise;

namespace Tortoise
{
    internal interface IHandleTextCommand
    {
        public bool HandleTextCommand(TortoiseBot tortoiseBot, SocketCommandContext commandContext, string[] splitContent);
    }
}
