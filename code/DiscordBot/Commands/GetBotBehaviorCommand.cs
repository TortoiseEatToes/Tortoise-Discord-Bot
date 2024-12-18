﻿
using System.Collections.Generic;
using Discord.Commands;
using Discord.WebSocket;

namespace Tortoise
{
    internal class GetBotBehaviorCommand  : TortoiseBotCommand, IHandleTextCommand
    {
        public override string GetDisplayName()
        {
            return "Get Bot Behavior";
        }
        
        public override string GetDescription()
        {
            return "Get Bot Behavior";
        }

        public bool HandleTextCommand(TortoiseBot tortoiseBot, SocketCommandContext commandContext, string[] splitContent)
        {
            if (splitContent[0].ToLower() != "get-behavior")
            {
                return false;
            }
            SendMessageToChannel(tortoiseBot, commandContext.Channel);
            return true;
        }
        
        private void SendMessageToChannel(TortoiseBot tortoiseBot, ISocketMessageChannel socketMessageChannel)
        {
            string outputMessage = "";

            List<string> behaviors = tortoiseBot.GetBehaviorPossible();
            foreach (string behavior in behaviors)
            {
                outputMessage += behavior + "\n";
            }

            socketMessageChannel.SendMessageAsync(outputMessage);
        }
    }
}