
using System;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Tortoise
{
    internal class RemoveReactionCommand  : TortoiseBotCommand, IHandleTextCommand
    {
        public override string GetDisplayName()
        {
            return "Start Unity Process";
        }
        
        public override string GetDescription()
        {
            return "Start Unity Process";
        }

        public bool HandleTextCommand(TortoiseBot tortoiseBot, SocketCommandContext commandContext, string[] splitContent)
        {
            if (splitContent[0].ToLower() != "reaction-remove")
            {
                return false;
            }
            
            ulong textChannelId = Convert.ToUInt64(splitContent[1]);
            SocketTextChannel socketTextChannel = commandContext.Guild.GetTextChannel(textChannelId);
            if (socketTextChannel == null)
            {
                return true;
            }

            ulong messageID = Convert.ToUInt64(splitContent[2]);
            string reaction = splitContent[3];
            if (!TryRemoveReaction(tortoiseBot, socketTextChannel, messageID, reaction))
            {
                Logger.WriteLine_Error("Failed to remove reaction");
            }

            return true;
        }

        private bool TryRemoveReaction(TortoiseBot tortoiseBot, SocketTextChannel socketTextChannel, ulong messageId, string reaction)
        {
            IMessage message = socketTextChannel.GetMessageAsync(messageId).Result;
            if (message == null)
            {
                return false;
            }
            
            if (Emote.TryParse(reaction, out Emote emote))
            {
                message.RemoveReactionAsync(emote, tortoiseBot.GetSocketClient().CurrentUser).Wait();
                return true;
            }
            
            if (Emoji.TryParse(reaction, out Emoji emoji))
            {
                message.RemoveReactionAsync(emoji, tortoiseBot.GetSocketClient().CurrentUser).Wait();
                return true;
            }

            return false;
        }
    }
}