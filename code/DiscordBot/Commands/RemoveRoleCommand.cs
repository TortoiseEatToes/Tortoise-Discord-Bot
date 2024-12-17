using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tortoise;
using TortoiseDiscordBot.code.DiscordBot.Commands.Interfaces;

namespace TortoiseDiscordBot.code.DiscordBot.Commands
{
    internal class RemoveRoleCommand : TortoiseBotCommand, IHandleReactionRemoved
    {
        public override string GetDescription()
        {
            return "If a user removes a react from the Roles message with a set reaction, remove the corresponding role.";
        }

        public override string GetDisplayName()
        {
            return "Remove Role";
        }

        public bool HandleReactionRemoved(TortoiseBot tortoiseBot, Cacheable<IUserMessage, ulong> arg1, Cacheable<IMessageChannel, ulong> arg2, SocketReaction arg3)
        {
            bool bHandled = false;
            if (arg1.Id == tortoiseBot.GetSettings().reactMessage)
            {
                Logger.WriteLine_Debug(arg3.User.Value.Username + " wants to remove: " + arg3.Emote.Name);
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
                    Logger.WriteLine_Debug("Ignoring reaction removed from " + arg3.User.Value.Username + " because they are a bot.");
                    return false;
                }
                ulong roleID;
                if (tortoiseBot.GetSettings().reactRoles.TryGetValue(arg3.Emote.Name, out roleID))
                {
                    if (DiscordBotUtilities.UserHasRole(guildUser, roleID))
                    {
                        Logger.WriteLine_Debug($"Removing role '{arg3.Emote.Name}' from user '{arg3.User.Value.Username}'");
                        guildUser.RemoveRoleAsync(roleID).Wait();
                        bHandled = true;
                    }
                }
            }
            return bHandled;
        }
    }
}
