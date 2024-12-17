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
    internal interface IHandleReactionRemoved
    {
        public bool HandleReactionRemoved(TortoiseBot tortoiseBot, Cacheable<IUserMessage, ulong> userMessage, Cacheable<IMessageChannel, ulong> messageChannel, SocketReaction socketReaction);
    }
}
