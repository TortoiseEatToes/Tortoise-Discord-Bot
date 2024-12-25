using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tortoise
{
    class HandleGetBehaviorMessage : OnMessageReceivedHandler
    {
        public override async Task<bool> Handle(TortoiseBot tortoiseBot, SocketCommandContext commandContext, string[] splitContent)
        {
            bool bHandled = false;
            if (splitContent[0].ToLower() == "get-behavior")
            {
                bHandled = true;
                await SendMessageToChannel(tortoiseBot, commandContext.Channel);
            }
            return bHandled;
        }

        private async Task SendMessageToChannel(TortoiseBot tortoiseBot, ISocketMessageChannel socketMessageChannel)
        {
            string outputMessage = "";

            List<TortoiseBotCommand> commands = tortoiseBot.GetCommands();
            foreach (TortoiseBotCommand command in commands)
            {
                outputMessage += command.GetDisplayName();
                outputMessage += $"\n   -{command.GetDescription()}\n";
            }

            await socketMessageChannel.SendMessageAsync(outputMessage);
        }

        public override string GetDescription()
        {
            return "Converts the current time to a specific time zone";
        }
    }
}
