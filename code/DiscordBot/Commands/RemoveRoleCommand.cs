using Discord;
using Discord.WebSocket;

namespace Tortoise
{
    internal class RemoveRoleCommand : TortoiseBotCommand, IHandleReactionRemoved
    {
        public override string GetDisplayName()
        {
            return "Remove Role";
        }
        
        public override string GetDescription()
        {
            return "If a user removes a react from the Roles message with a set reaction, remove the corresponding role.";
        }
        
        public bool HandleReactionRemoved(TortoiseBot tortoiseBot, Cacheable<IUserMessage, ulong> userMessage, Cacheable<IMessageChannel, ulong> messageChannel, SocketReaction socketReaction)
        {
            if (userMessage.Id != tortoiseBot.GetSettings().reactMessage)
            {
                return false;
            }

            string result;
            if (TryRemoveRole(tortoiseBot, socketReaction.User.Value, socketReaction.Emote, out result))
            {
                Logger.WriteLine_Debug(result);
            }
            else
            {
                Logger.WriteLine_Error($"Failed to remove role because {result}");
            }
            return true;
        }

        private bool TryRemoveRole(TortoiseBot tortoiseBot, IUser user, IEmote emote, out string reason)
        {
            IGuildUser? guildUser = user as IGuildUser;
            if (guildUser is null)
            {
                reason = "User is not a guild user";
                return false;
            }
            
            if (tortoiseBot.GuildUserIsSelf(guildUser))
            {
                reason = "User is self";
                return false;
            }
            
            if (guildUser.IsBot)
            {
                reason = "User is bot";
                return false;
            }
            
            ulong roleId;
            if (!tortoiseBot.GetSettings().reactRoles.TryGetValue(emote.Name, out roleId))
            {
                reason = "This emote is not a react role";
                return false;
            }

            if (!DiscordBotUtilities.UserHasRole(guildUser, roleId))
            {
                reason = "This user doesn't have a react role";
                return false;
            }
            
            guildUser.RemoveRoleAsync(roleId).Wait();
            reason = $"Removed role '{emote.Name}' from user '{user.Username}'";
            return true;
        }
    }
}
