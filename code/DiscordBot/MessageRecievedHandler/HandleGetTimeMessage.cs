using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tortoise
{
    class HandleGetTimeMessage : OnMessageReceivedHandler
    {
        public override async Task<bool> Handle(TortoiseBot tortoiseBot, SocketCommandContext commandContext, string[] splitContent)
        {
            bool bHandled = false;
            if (splitContent[0].ToLower() == "get-time")
            {
                TimeZoneInfo timeZoneInfo = TimeZoneInfo.Local;
                if (splitContent.Length > 1)
                {
                    string timeZoneId = "";
                    for(int i = 1; i < splitContent.Length; i++)
                    {
                        timeZoneId += splitContent[i] + " ";
                    }
                    timeZoneId = timeZoneId.Trim();
                    
                    try
                    {
                        timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                    }
                    catch(Exception exception)
                    {
                        Logger.WriteLine($"Failed to find time zone '{timeZoneId}' due to exception: {exception}");
                        tortoiseBot.PostMessageToDefaultChannel($"Failed to find time zone: {timeZoneId}");
                    }
                }

                DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);
                string outputMessage = $"Current time at timezone {timeZoneInfo.ToString()} is {currentTime.ToString("HH:mm")}";
                commandContext.Channel.SendMessageAsync(outputMessage).Wait();
                bHandled = true;
            }
            return bHandled;
        }

        public override string GetDescription()
        {
            return "Converts the current time to a specific time zone";
        }
    }
}
