using Discord.WebSocket;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tortoise;

namespace Tortoise
{
    internal interface IHandleReactionAdded
    {
        bool Handle(TortoiseBot tortoiseBot, Cacheable<IUserMessage, ulong> arg1, Cacheable<IMessageChannel, ulong> arg2, SocketReaction socketReaction);
    }
}
