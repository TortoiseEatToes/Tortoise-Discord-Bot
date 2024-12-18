
using Discord;
using Discord.WebSocket;

namespace Tortoise
{
    internal class AddRoleCommand : TortoiseBotCommand, IHandleReactionAdded
    {
        public override string GetDisplayName()
        {
            return "Add Role";
        }
        
        public override string GetDescription()
        {
            return "Add a role to a user";
        }

        public bool Handle(TortoiseBot tortoiseBot, Cacheable<IUserMessage, ulong> arg1, Cacheable<IMessageChannel, ulong> arg2, SocketReaction socketReaction)
        {
            if (arg1.Id != tortoiseBot.GetSettings().reactMessage)
            {
                return false;
            }
            
            if (!tortoiseBot.GetSettings().reactRoles.TryGetValue(socketReaction.Emote.Name, out ulong roleId))
            {
                if (TryAddRole(tortoiseBot, socketReaction.User.Value, roleId))
                {
                    Logger.WriteLine_Debug("Added role to user");
                }
                else
                {
                    Logger.WriteLine_Error("Failed to add role to user");
                }
            }
            else
            {
                Logger.WriteLine_Error("Failed to add role to user");
            }

            return true;
        }

        private bool TryAddRole(TortoiseBot tortoiseBot, IUser user, ulong roleId)
        {
            IGuildUser? guildUser = user as IGuildUser;
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
                Logger.WriteLine_Debug("Ignoring reaction added from " + user.Username + " because they are a bot.");
                return false;
            }
            
            if (DiscordBotUtilities.UserHasRole(guildUser, roleId))
            {
                return false;
            }
            
            guildUser.AddRoleAsync(roleId);
            return true;
        }
    }
}