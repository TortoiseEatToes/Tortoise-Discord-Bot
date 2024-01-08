using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tortoise
{
    class HandleUnityMessage : OnMessageReceivedHandler
    {
        public override async Task<bool> Handle(TortoiseBot tortoiseBot, SocketCommandContext commandContext, string[] splitContent)
        {
            bool bHandled = false;
            if (splitContent[0].ToLower() == "unity")
            {
                bHandled = true;
                string userTag = $"<@{commandContext.User.Id}>";

                IGuildUser? guildUser = commandContext.User as IGuildUser;
                if (guildUser == null)
                {
                    tortoiseBot.PostMessageToDefaultChannel($"Sorry, {userTag}, you cannot use this command in that context.");
                    return bHandled;
                }

                if (!DiscordBotUtilities.UserHasRole(guildUser, tortoiseBot.GetSettings().botOperator))
                {
                    tortoiseBot.PostMessageToDefaultChannel($"Sorry, {userTag}, you do not have permissions to use this command.");
                    return bHandled;
                }

                if (splitContent.Length < 4)
                {
                    tortoiseBot.PostMessageToDefaultChannel($"Sorry, {userTag}, this command requires at least 3 arguments after 'Unity'.");
                    return bHandled;
                }

                string projectName = splitContent[1];
                string? projectPath;
                if (!tortoiseBot.GetSettings().unityProjects.TryGetValue(projectName, out projectPath))
                {
                    tortoiseBot.PostMessageToDefaultChannel($"Sorry, {userTag}, failed to find Unity project: {projectName}!");
                    return bHandled;
                }
                string unityVersion = splitContent[2];
                string unityMethod = splitContent[3];

                string additionalArguments = "";
                if (splitContent.Length > 4)
                {
                    for (int i = 4; i < splitContent.Length; ++i)
                    {
                        additionalArguments += $"{splitContent[i]} ";
                    }
                }
                additionalArguments.Trim();

                StartUnityProcess(tortoiseBot, projectName, projectPath, unityVersion, unityMethod, additionalArguments);
            }
            return bHandled;
        }

        private void StartUnityProcess(TortoiseBot tortoiseBot, string projectName, string projectPath, string unityVersion, string unityMethod, string additionalArgs)
        {
            string command = $"\"{tortoiseBot.GetSettings().unityExecutableRoot}\\{unityVersion}\\Editor\\Unity.exe\" -quit -batchmode -logFile - -projectPath \"{projectPath}\" -executeMethod {unityMethod} {additionalArgs}";
            tortoiseBot.StartSideProcess(command, $"Unity-{projectName}-{unityVersion}-{unityMethod}");
        }

        public override string GetDescription()
        {
            return "Starts a Unity process";
        }
    }
}
