using Discord;
using Discord.WebSocket;

namespace Tortoise
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

        public bool HandleReactionRemoved(TortoiseBot tortoiseBot, Cacheable<IUserMessage, ulong> userMessage, Cacheable<IMessageChannel, ulong> messageChannel, SocketReaction socketReaction)
        {
            bool bHandled = false;
            if (userMessage.Id == tortoiseBot.GetSettings().reactMessage)
            {
                Logger.WriteLine_Debug(socketReaction.User.Value.Username + " wants to remove: " + socketReaction.Emote.Name);
                IGuildUser? guildUser = socketReaction.User.Value as IGuildUser;
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
                    Logger.WriteLine_Debug("Ignoring reaction removed from " + socketReaction.User.Value.Username + " because they are a bot.");
                    return false;
                }
                ulong roleID;
                if (tortoiseBot.GetSettings().reactRoles.TryGetValue(socketReaction.Emote.Name, out roleID))
                {
                    if (DiscordBotUtilities.UserHasRole(guildUser, roleID))
                    {
                        Logger.WriteLine_Debug($"Removing role '{socketReaction.Emote.Name}' from user '{socketReaction.User.Value.Username}'");
                        guildUser.RemoveRoleAsync(roleID).Wait();
                        bHandled = true;
                    }
                }
            }
            return bHandled;
        }
    }
}
