using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tortoise;

namespace TortoiseDiscordBot.code.DiscordBot.Commands.Interfaces
{
    internal interface IHandleTextCommand
    {
        public Task<bool> HandleTextCommand(TortoiseBot tortoiseBot, SocketCommandContext commandContext, string[] splitContent);
    }
}
