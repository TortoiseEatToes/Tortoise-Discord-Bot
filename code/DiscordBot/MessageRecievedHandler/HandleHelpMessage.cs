using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tortoise
{
    class HandleHelpMessage : OnMessageReceivedHandler
    {
        public override async Task<bool> Handle(TortoiseBot tortoiseBot, SocketCommandContext commandContext, string[] splitContent)
        {
            bool bHandled = false;
            if (splitContent[0].ToLower() == "help")
            {
                string outputMessage = "";
                outputMessage += "Hello!  I'm TortoiseBot!  I'm here to help with game jams!\n";
                outputMessage += "- Use '!unity' to use the Unity command with the args: !unity projectName unityVersion unityMethod additionalArguments\n";
                string rolesChannel = "<#983601765628919809>";
                outputMessage += $"- You can also react to react to the roles message in {rolesChannel} to get roles.\n";

                await commandContext.Channel.SendMessageAsync(outputMessage);
                bHandled = true;
            }
            return bHandled;
        }

        public override string GetDescription()
        {
            return "Gets the public facing description of what this bot can do";
        }
    }
}
