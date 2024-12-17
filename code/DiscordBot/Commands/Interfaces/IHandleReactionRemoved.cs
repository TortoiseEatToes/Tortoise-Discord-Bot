using Discord.WebSocket;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tortoise;

namespace TortoiseDiscordBot.code.DiscordBot.Commands.Interfaces
{
    internal interface IHandleReactionRemoved
    {
        bool HandleReactionRemoved(TortoiseBot tortoiseBot, Cacheable<IUserMessage, ulong> arg1, Cacheable<IMessageChannel, ulong> arg2, SocketReaction arg3);
    }
}
