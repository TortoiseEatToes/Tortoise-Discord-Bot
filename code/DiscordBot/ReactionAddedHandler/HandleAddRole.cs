using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace Tortoise
{
    class HandleAddRole : OnReactionAddedHandler
    {
        public override async Task<bool> Handle(TortoiseBot tortoiseBot, Cacheable<IUserMessage, ulong> arg1, Cacheable<IMessageChannel, ulong> arg2, SocketReaction arg3)
        {
            bool bHandled = false;
            if (arg1.Id == tortoiseBot.GetSettings().reactMessage)
            {
                Logger.WriteLine_Debug(arg3.User.Value.Username + " wants to add: " + arg3.Emote.Name);
                IGuildUser? guildUser = arg3.User.Value as IGuildUser;
                if(guildUser is null)
                {
                    return false;
                }
                if (tortoiseBot.GuildUserIsSelf(guildUser))
                {
                    return false;
                }
                if (guildUser.IsBot)
                {
                    Logger.WriteLine_Debug("Ignoring reaction added from " + arg3.User.Value.Username + " because they are a bot.");
                    return false;
                }
                ulong roleID;
                if (tortoiseBot.GetSettings().reactRoles.TryGetValue(arg3.Emote.Name, out roleID))
                {
                    if (!DiscordBotUtilities.UserHasRole(guildUser, roleID))
                    {
                        Logger.WriteLine_Debug($"Giving role '{arg3.Emote.Name}' to user '{arg3.User.Value.Username}'");
                        await guildUser.AddRoleAsync(roleID);
                        bHandled = true;
                    }
                }
            }

            return bHandled;
        }

        public override string GetDescription()
        {
            return "If a user adds a react from the Roles message with a set reaction, add the corresponding role.";
        }
    }
}
