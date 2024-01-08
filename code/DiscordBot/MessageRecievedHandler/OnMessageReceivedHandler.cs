using Discord.Commands;
using System.Threading.Tasks;

namespace Tortoise
{
    abstract class OnMessageReceivedHandler
    {
        public virtual async Task<bool> Handle(TortoiseBot tortoiseBot, SocketCommandContext commandContext, string[] splitContent)
        {
            return await Task.FromResult(false);
        }

        public abstract string GetDescription();
    }
}
