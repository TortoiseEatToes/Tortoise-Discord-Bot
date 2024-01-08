using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace Tortoise
{
    abstract class OnReactionRemovedHandler
    {
        public virtual async Task<bool> Handle(TortoiseBot tortoiseBot, Cacheable<IUserMessage, ulong> arg1, Cacheable<IMessageChannel, ulong> arg2, SocketReaction arg3)
        {
            return await Task.FromResult(false);
        }

        public abstract string GetDescription();
    }
}