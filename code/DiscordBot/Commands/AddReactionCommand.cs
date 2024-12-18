
using System;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Tortoise
{
    internal class AddReactionCommand  : TortoiseBotCommand, IHandleTextCommand
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
            if (splitContent[0].ToLower() != "reaction-add")
            {
                return false;
            }
            
            ulong textChannelId = Convert.ToUInt64(splitContent[1]);
            SocketTextChannel socketTextChannel = commandContext.Guild.GetTextChannel(textChannelId);
            if (socketTextChannel == null)
            {
                return true;
            }

            ulong messageId = Convert.ToUInt64(splitContent[2]);
            string reaction = splitContent[3];
            if (!TryAddReaction(tortoiseBot, socketTextChannel, messageId, reaction))
            {
                Logger.WriteLine_Error("Failed to add reaction");
            }

            return true;
        }

        private bool TryAddReaction(TortoiseBot tortoiseBot, SocketTextChannel socketTextChannel, ulong messageId, string reaction)
        {
            IMessage message = socketTextChannel.GetMessageAsync(messageId).Result;
            if (message == null)
            {
                return false;
            }
            
            if (Emote.TryParse(reaction, out Emote emote))
            {
                message.AddReactionAsync(emote).Wait();
                return true;
            }
            
            if (Emoji.TryParse(reaction, out Emoji emoji))
            {
                message.AddReactionAsync(emoji).Wait();
                return true;
            }

            return true;
        }
    }
}