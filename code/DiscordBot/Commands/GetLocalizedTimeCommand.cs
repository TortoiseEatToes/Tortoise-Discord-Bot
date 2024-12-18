
using System;
using System.Security;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace Tortoise
{
    internal class GetLocalizedTimeCommand  : TortoiseBotCommand, IHandleTextCommand
    {
        public override string GetDisplayName()
        {
            return "Get Localized Time";
        }
        
        public override string GetDescription()
        {
            return "Get Localized Time";
        }

        public bool HandleTextCommand(TortoiseBot tortoiseBot, SocketCommandContext commandContext, string[] splitContent)
        {
            if (splitContent[0].ToLower() != "get-time")
            {
                return false;
            }
            
            TimeZoneInfo timeZoneInfo = TimeZoneInfo.Local;
            if (splitContent.Length > 1)
            {
                string timeZoneId = "";
                for(int i = 1; i < splitContent.Length; i++)
                {
                    timeZoneId += splitContent[i] + " ";
                }
                timeZoneId = timeZoneId.Trim();
                if (!TryGetTimeZoneFromString(timeZoneId, ref timeZoneInfo, out string? failureReason))
                {
                    string outputMessage = $"Failed to get localized time for id: {timeZoneId} due to {failureReason}";
                    commandContext.Channel.SendMessageAsync(outputMessage).Wait();
                    return true;
                }
            }
            
            SendTimeMessage(timeZoneInfo, commandContext.Channel);
            return true;
        }

        private bool TryGetTimeZoneFromString(string timeZoneId, ref TimeZoneInfo timeZoneInfo, out string? failureReason)
        {
            try
            {
                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                failureReason = null;
                return true;
            }
            catch (ArgumentNullException exception)
            {
                failureReason = exception.Message;
                return false;
            }
            catch (InvalidTimeZoneException exception)
            {
                failureReason = exception.Message;
            }
            catch (SecurityException exception)
            {
                failureReason = exception.Message;
            }
            catch (TimeZoneNotFoundException exception)
            {
                failureReason = exception.Message;
            }
            return false;
        }

        private void SendTimeMessage(TimeZoneInfo timeZoneInfo, ISocketMessageChannel channel)
        {
            DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);
            string outputMessage = $"Current time at timezone {timeZoneInfo} is {currentTime:HH:mm}";
            channel.SendMessageAsync(outputMessage).Wait();
        }
    }
}