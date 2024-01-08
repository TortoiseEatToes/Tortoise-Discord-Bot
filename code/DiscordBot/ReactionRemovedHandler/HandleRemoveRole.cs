using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tortoise
{
    class HandleRemoveRole : OnReactionRemovedHandler
    {
        public override async Task<bool> Handle(TortoiseBot tortoiseBot, Cacheable<IUserMessage, ulong> arg1, Cacheable<IMessageChannel, ulong> arg2, SocketReaction arg3)
        {
            bool bHandled = false;
            if (arg1.Id == tortoiseBot.GetSettings().reactMessage)
            {
                Logger.WriteLine(arg3.User.Value.Username + " wants to remove: " + arg3.Emote.Name);
                IGuildUser? guildUser = arg3.User.Value as IGuildUser;
                if (guildUser is null)
                {
                    return false;
                }
                if (tortoiseBot.GuildUserIsSelf(guildUser))
                {
                    return false;
                }
                if (guildUser.IsBot)
                {
                    Logger.WriteLine("Ignoring reaction removed from " + arg3.User.Value.Username + " because they are a bot.");
                    return false;
                }
                ulong roleID;
                if (tortoiseBot.GetSettings().reactRoles.TryGetValue(arg3.Emote.Name, out roleID))
                {
                    if (DiscordBotUtilities.UserHasRole(guildUser, roleID))
                    {
                        Logger.WriteLine($"Removing role '{arg3.Emote.Name}' from user '{arg3.User.Value.Username}'");
                        await guildUser.RemoveRoleAsync(roleID);
                        bHandled = true;
                    }
                }
            }
            return bHandled;
        }

        public override string GetDescription()
        {
            return "If a user removes a react from the Roles message with a set reaction, remove the corresponding role.";
        }
    }
}
