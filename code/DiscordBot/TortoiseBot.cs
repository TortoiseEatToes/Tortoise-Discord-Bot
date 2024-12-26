using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Tortoise
{
    public class TortoiseBot : DiscordBot
    {
        private List<TortoiseBotCommand> commands = new List<TortoiseBotCommand>();
        private List<IHandleSlashCommand> slashCommandHandlers = new List<IHandleSlashCommand>();
        private List<IHandleTextCommand> textCommandHandlers = new List<IHandleTextCommand>();
        private List<IHandleReactionAdded> reactionAddedHandlers = new List<IHandleReactionAdded>();
        private List<IHandleReactionRemoved> reactionRemovedHandlers = new List<IHandleReactionRemoved>();

        private List<SideProcess> sideProcesses = new List<SideProcess>();
        private Mutex mutexSideProcess = new Mutex();

        private TortoiseBotSettings settings = new TortoiseBotSettings();

        #region BehaviorSettings
        public TortoiseBot()
        {
            settings = new TortoiseBotSettings();
        }
        public TortoiseBot(BotRuntimeSettings botSettings)
        {
            ReadFromSettings(botSettings);
        }

        ~TortoiseBot()
        {
            SaveSettingsToDefaultFile();
        }

        public void Run(BotStartupSettings botStartupSettings)
        {
            this.Run(botStartupSettings.json.token).GetAwaiter().GetResult();
        }

        public void ReadFromSettings(BotRuntimeSettings botSettings)
        {
            settings = botSettings.json.tortoiseBotSettings;
            foreach(string commandName in botSettings.json.commands)
            {
                Type? type = Type.GetType(commandName);
                if (type is null)
                {
                    continue;
                }
                object? objectInstance = Activator.CreateInstance(type);
                if (objectInstance is null)
                {
                    continue;
                }
                TortoiseBotCommand? command = objectInstance as TortoiseBotCommand;
                if (command is null)
                {
                    continue;
                }
                commands.Add(command);
                TryAddHandler<IHandleSlashCommand>(slashCommandHandlers, command);
                TryAddHandler<IHandleTextCommand>(textCommandHandlers, command);
                TryAddHandler<IHandleReactionAdded>(reactionAddedHandlers, command);
                TryAddHandler<IHandleReactionRemoved>(reactionRemovedHandlers, command);
            }
        }

        public void WriteToSettings(BotRuntimeSettings botSettings)
        {
            botSettings.json.tortoiseBotSettings = settings;
            PopulateListWithObjectNames<TortoiseBotCommand>(botSettings.json.commands, commands);
        }

        public void SaveSettingsToDefaultFile()
        {
            SaveSettingsToFile(settings.saveFilePath);
        }

        public void SaveSettingsToFile(string filePath)
        {
            BotRuntimeSettings botSettings = new BotRuntimeSettings();
            WriteToSettings(botSettings);
            botSettings.WriteToFile(filePath);
        }

        private static void PopulateListWithObjectNames<TypeName>(List<string> listOfObjectNames, List<TypeName> listOfObjects)
        {
            listOfObjectNames.Clear();
            foreach (object? objectInstance in listOfObjects)
            {
                if(objectInstance is not null)
                {
                    listOfObjectNames.Add(objectInstance.GetType().ToString());
                }
            }
        }

        private void TryAddHandler<TypeName>(List<TypeName> handlerList, object hanlderObject) where TypeName : class
        {
            TypeName? handler = hanlderObject as TypeName;
            if (handler is null)
            {
                return;
            }
            handlerList.Add(handler);
        }

        public List<TortoiseBotCommand> GetCommands()
        {
            return commands;
        }
        #endregion

        #region EventHandlers
        protected override async Task OnMessageReceived(SocketMessage socketMessage)
        {
            Logger.WriteLine_Debug("OnMessageReceived: " + socketMessage.ToString());

            SocketUserMessage? userMessage = socketMessage as SocketUserMessage;
            if(userMessage is null)
            {
                return;
            }

            SocketCommandContext commandContext = new SocketCommandContext(m_DiscordSocketClient, userMessage);
            if (commandContext.User.IsBot)
            {
                return;
            }

            if (userMessage.Content.StartsWith(settings.messageRecievedCharacter))
            {
                string[] splitContent = userMessage.Content.Substring(1).Split(" ");
                foreach (IHandleTextCommand textCommandHandler in textCommandHandlers)
                {
                    if (textCommandHandler.HandleTextCommand(this, commandContext, splitContent))
                    {
                        break;
                    }
                }
            }
        }

        protected override async Task OnReactionAdded(Cacheable<IUserMessage, ulong> arg1, Cacheable<IMessageChannel, ulong> arg2, SocketReaction arg3)
        {
            Logger.WriteLine_Debug("OnReactionAdded");
            foreach (IHandleReactionAdded onReactionAddedHandler in reactionAddedHandlers)
            {
                if (onReactionAddedHandler.Handle(this, arg1, arg2, arg3))
                {
                    break;
                }
            }
        }

        protected override async Task OnReactionRemoved(Cacheable<IUserMessage, ulong> arg1, Cacheable<IMessageChannel, ulong> arg2, SocketReaction arg3)
        {
            Logger.WriteLine_Debug("OnReactionRemoved");
            foreach (IHandleReactionRemoved onReactionRemovedHandler in reactionRemovedHandlers)
            {
                if (onReactionRemovedHandler.HandleReactionRemoved(this, arg1, arg2, arg3))
                {
                    break;
                }
            }
        }

        public async void PostMessageToDefaultChannel(String outputMessage)
        {
            IMessageChannel? botLogsChannel = GetSocketClient().GetChannel(settings.defaultOutputChannel) as IMessageChannel;
            if(botLogsChannel is null)
            {
                Logger.WriteLine_Error($"PostMessageToDefaultChannel - Failed to find botLogsChannel at {settings.defaultOutputChannel} to post message to: {outputMessage}");
                return;
            }

            await botLogsChannel.SendMessageAsync(outputMessage);
        }
        #endregion

        #region Initialization
        protected override async Task OnClientIsReady()
        {
            Logger.WriteLine_Debug("OnClientIsReady - Start");

            await InitializeRoles();

            Logger.WriteLine_Debug("OnClientIsReady - End");
        }

        private async Task InitializeRoles()
        {
            SocketGuild socketGuild = m_DiscordSocketClient.GetGuild(settings.server_id_tortoise_jams);
            if (socketGuild is null)
            {
                return;
            }
            SocketTextChannel socketTextChannel = socketGuild.GetTextChannel(settings.reactChannel);
            if (socketTextChannel is null)
            {
                return;
            }
            IMessage messageRoles = await socketTextChannel.GetMessageAsync(settings.reactMessage);
            if (messageRoles is null)
            {
                return;
            }

            await InitializeRoles(messageRoles, socketGuild);
        }

        private async Task InitializeRoles(IMessage messageRoles, SocketGuild socketGuild)
        {
            foreach (KeyValuePair<IEmote, ReactionMetadata> reactionData in messageRoles.Reactions)
            {
                ulong roleID;
                if (settings.reactRoles.TryGetValue(reactionData.Key.Name, out roleID))
                {
                    IAsyncEnumerable<IReadOnlyCollection<IUser>> userCollections = messageRoles.GetReactionUsersAsync(reactionData.Key, 100);
                    await foreach (IReadOnlyCollection<IUser> users in userCollections)
                    {
                        foreach (IUser user in users)
                        {
                            IGuildUser guildUser = socketGuild.GetUser(user.Id);
                            if(guildUser != null)
                            {
                                if (GuildUserIsSelf(guildUser))
                                {
                                    //Do nothing
                                }
                                else if (guildUser.IsBot)
                                {
                                    Logger.WriteLine_Debug($"Skipping user '{user.Username}' for role initialization because they are a bot");
                                }
                                else
                                {
                                    if (!DiscordBotUtilities.UserHasRole(guildUser, roleID))
                                    {
                                        Logger.WriteLine_Info($"Giving role '{reactionData.Key.Name}' to user '{user.Username}'");
                                        await guildUser.AddRoleAsync(roleID);
                                    }
                                }
                            }
                            else
                            {
                                Logger.WriteLine_Warning($"Cannot give '{reactionData.Key.Name}' to '{user.Username}' because they are missing");
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region SideProcess
        public void StartSideProcess(string command, string name)
        {
            SideProcess sideProcess = new SideProcess(name);
            sideProcess.Start(command, OnProcessComplete);
            mutexSideProcess.WaitOne();
            sideProcesses.Add(sideProcess);
            mutexSideProcess.ReleaseMutex();
        }

        private void OnProcessComplete(SideProcess sideProcess)
        {
            PostMessageToDefaultChannel($"SideProcess '{sideProcess.GetName()}' finished with code: {sideProcess.GetResult()}");
            mutexSideProcess.WaitOne();
            sideProcesses.Remove(sideProcess);
            mutexSideProcess.ReleaseMutex();
        }
        #endregion

        #region Accessors

        public void SetDefaultChannelToDebugChannel()
        {
            settings.defaultOutputChannel = settings.debugChannel;
        }

        public void SetDefaultChannelToLogChannel()
        {
            settings.defaultOutputChannel = settings.logsChannel;
        }

        public TortoiseBotSettings GetSettings()
        {
            return settings;
        }
        #endregion
    }
}
