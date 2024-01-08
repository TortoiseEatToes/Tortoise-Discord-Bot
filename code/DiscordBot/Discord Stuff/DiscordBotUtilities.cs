using Discord;

namespace Tortoise
{
    class DiscordBotUtilities
    {
        public static bool UserHasRole(IGuildUser guildUser, ulong roleID)
        {
            foreach (ulong ownedRoleID in guildUser.RoleIds)
            {
                if (ownedRoleID == roleID)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
