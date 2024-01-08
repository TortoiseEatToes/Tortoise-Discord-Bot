using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tortoise
{
    class HandleReactionRemoveMessage : OnMessageReceivedHandler
    {
        public override async Task<bool> Handle(TortoiseBot tortoiseBot, SocketCommandContext commandContext, string[] splitContent)
        {
            bool bHandled = false;
            if (splitContent[0].ToLower() == "reaction-remove")
            {
                ulong textChannelID = Convert.ToUInt64(splitContent[1]);
                SocketTextChannel socketTextChannel = commandContext.Guild.GetTextChannel(textChannelID);
                if (socketTextChannel != null)
                {
                    ulong messageID = Convert.ToUInt64(splitContent[2]);
                    IMessage message = socketTextChannel.GetMessageAsync(messageID).Result;
                    if (message != null)
                    {
                        string escapedEmote = splitContent[3];
                        Emote emote;
                        if (Emote.TryParse(escapedEmote, out emote))
                        {
                            message.RemoveReactionAsync(emote, tortoiseBot.GetSocketClient().CurrentUser).Wait();
                        }
                        else
                        {
                            Emoji emoji;
                            if (Emoji.TryParse(escapedEmote, out emoji))
                            {
                                message.RemoveReactionAsync(emoji, tortoiseBot.GetSocketClient().CurrentUser).Wait();
                            }
                        }
                    }
                }
                bHandled = true;
            }
            return bHandled;
        }

        public override string GetDescription()
        {
            return "Allows you to remove a reaction through text";
        }
    }
}
