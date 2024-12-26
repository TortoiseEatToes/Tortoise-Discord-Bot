using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Tortoise
{
    public class HelpCommand : TortoiseBotCommand, IHandleTextCommand
    {
        public override string GetDisplayName()
        {
            return "Remove Role";
        }

        public override string GetDescription()
        {
            return "If a user removes a react from the Roles message with a set reaction, remove the corresponding role.";
        }

        public bool HandleTextCommand(TortoiseBot tortoiseBot, SocketCommandContext commandContext, string[] splitContent)
        {
            bool bHandled = false;
            if (splitContent[0].ToLower() == "help")
            {
                string outputMessage = "";
                outputMessage += "Hello!  I'm TortoiseBot!  I'm here to help with game jams!\n";
                outputMessage += "- Use '!unity' to use the Unity command with the args: !unity projectName unityVersion unityMethod additionalArguments\n";
                string rolesChannel = $"<#{tortoiseBot.GetSettings().reactChannel}>";
                outputMessage += $"- You can also react to react to the roles message in {rolesChannel} to get roles.\n";

                commandContext.Channel.SendMessageAsync(outputMessage);
                bHandled = true;
            }
            return bHandled;
        }
    }
}
