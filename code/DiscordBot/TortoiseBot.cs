using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tortoise
{
    public class TortoiseBot : DiscordBot
    {
        private List<OnMessageReceivedHandler> onMessageReceivedHandlers = new List<OnMessageReceivedHandler>();
        private List<OnReactionAddedHandler> onReactionAddedHandlers = new List<OnReactionAddedHandler>();
        private List<OnReactionRemovedHandler> onReactionRemovedHandlers = new List<OnReactionRemovedHandler>();
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
            PopulateListWithObjects<OnMessageReceivedHandler>(onMessageReceivedHandlers, botSettings.json.onMessageReceivedHandlers);
            PopulateListWithObjects<OnReactionAddedHandler>(onReactionAddedHandlers, botSettings.json.onReactionAddedHandlers);
            PopulateListWithObjects<OnReactionRemovedHandler>(onReactionRemovedHandlers, botSettings.json.onReactionRemovedHandlers);
        }

        public void WriteToSettings(BotRuntimeSettings botSettings)
        {
            botSettings.json.tortoiseBotSettings = settings;
            PopulateListWithObjectNames<OnMessageReceivedHandler>(botSettings.json.onMessageReceivedHandlers, onMessageReceivedHandlers);
            PopulateListWithObjectNames<OnReactionAddedHandler>(botSettings.json.onReactionAddedHandlers, onReactionAddedHandlers);
            PopulateListWithObjectNames<OnReactionRemovedHandler>(botSettings.json.onReactionRemovedHandlers, onReactionRemovedHandlers);
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

        private static void PopulateListWithObjects<TypeName>(List<TypeName> listOfObjects, List<string> listOfObjectNames) where TypeName : class
        {
            listOfObjects.Clear();
            foreach (string objectName in listOfObjectNames)
            {
                TypeName? typeNameInstance = GetObjectOfType<TypeName>(objectName);
                if (typeNameInstance != null)
                {
                    listOfObjects.Add(typeNameInstance);
                }
            }
        }

        private static TypeName? GetObjectOfType<TypeName>(string qualifiedName) where TypeName : class
        {
            TypeName? createdType = default(TypeName);
            Type? type = Type.GetType(qualifiedName);
            if (type != null)
            {
                object? typeInstance = Activator.CreateInstance(type);
                if(typeInstance is not null)
                {
                    TypeName? typeName = typeInstance as TypeName;
                    if (typeName is not null)
                    {
                        createdType = typeName;
                    }
                    else
                    {
                        Logger.WriteLine($"Failed to cast {qualifiedName} to {typeof(TypeName).FullName}");
                    }
                }
                else
                {
                    Logger.WriteLine($"Failed to create instance of type {type}");
                }
            }
            else
            {
                Logger.WriteLine($"Failed to find type for {qualifiedName}");
            }
            return createdType;
        }

        public List<string> GetBehaviorPossible()
        {
            List<string> behaviors = new List<string>();
            behaviors.Add("=== onMessageReceivedHandlers ===");
            foreach (OnMessageReceivedHandler onMessageReceivedHandler in onMessageReceivedHandlers)
            {
                behaviors.Add($"    -{onMessageReceivedHandler.GetType().ToString()}: {onMessageReceivedHandler.GetDescription()}");
            }
            behaviors.Add("=== onReactionAddedHandlers ===");
            foreach (OnReactionAddedHandler onReactionAddedHandler in onReactionAddedHandlers)
            {
                behaviors.Add($"    -{onReactionAddedHandler.GetType().ToString()}: {onReactionAddedHandler.GetDescription()}");
            }
            behaviors.Add("=== onReactionRemovedHandlers ===");
            foreach (OnReactionRemovedHandler onReactionRemovedHandler in onReactionRemovedHandlers)
            {
                behaviors.Add($"    -{onReactionRemovedHandler.GetType().ToString()}: {onReactionRemovedHandler.GetDescription()}");
            }
            return behaviors;
        }
        #endregion

        #region EventHandlers
        protected override async Task OnMessageReceived(SocketMessage socketMessage)
        {
            Logger.WriteLine("OnMessageReceived: " + socketMessage.ToString());

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
                foreach (OnMessageReceivedHandler onMessageReceivedHandler in onMessageReceivedHandlers)
                {
                    if (await onMessageReceivedHandler.Handle(this, commandContext, splitContent))
                    {
                        break;
                    }
                }
            }
        }

        protected override async Task OnReactionAdded(Cacheable<IUserMessage, ulong> arg1, Cacheable<IMessageChannel, ulong> arg2, SocketReaction arg3)
        {
            Logger.WriteLine("OnReactionAdded");
            foreach (OnReactionAddedHandler onReactionAddedHandler in onReactionAddedHandlers)
            {
                if (await onReactionAddedHandler.Handle(this, arg1, arg2, arg3))
                {
                    break;
                }
            }
        }

        protected override async Task OnReactionRemoved(Cacheable<IUserMessage, ulong> arg1, Cacheable<IMessageChannel, ulong> arg2, SocketReaction arg3)
        {
            Logger.WriteLine("OnReactionRemoved");
            foreach (OnReactionRemovedHandler onReactionRemovedHandler in onReactionRemovedHandlers)
            {
                if (await onReactionRemovedHandler.Handle(this, arg1, arg2, arg3))
                {
                    break;
                }
            }
        }

        public async void PostMessageToDefaultChannel(String outputMessage)
        {
            IMessageChannel? botLogsChannel = GetSocketClient().GetChannel(settings.defaultOutputChannel) as IMessageChannel;
            if(botLogsChannel is not null)
            {
                await botLogsChannel.SendMessageAsync(outputMessage);
            }
            else
            {
                Logger.WriteLine($"PostMessageToDefaultChannel - Failed to find botLogsChannel at {settings.defaultOutputChannel} to post message to: {outputMessage}");
            }
        }
        #endregion

        #region Initialization
        protected override async Task OnClientIsReady()
        {
            Logger.WriteLine("OnClientIsReady - Start");
            SocketGuild socketGuild = m_DiscordSocketClient.GetGuild(settings.server_id_tortoise_jams);
            if (socketGuild != null)
            {
                SocketTextChannel socketTextChannel = socketGuild.GetTextChannel(settings.reactChannel);
                if (socketTextChannel != null)
                {
                    IMessage messageRoles = await socketTextChannel.GetMessageAsync(settings.reactMessage);
                    if (messageRoles != null)
                    {
                        await InitializeRoles(messageRoles, socketGuild);
                    }
                }
            }
            Logger.WriteLine("OnClientIsReady - End");
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
                                    Logger.WriteLine($"Skipping user '{user.Username}' for role initialization because they are a bot");
                                }
                                else
                                {
                                    if (!DiscordBotUtilities.UserHasRole(guildUser, roleID))
                                    {
                                        Logger.WriteLine($"Giving role '{reactionData.Key.Name}' to user '{user.Username}'");
                                        await guildUser.AddRoleAsync(roleID);
                                    }
                                }
                            }
                            else
                            {
                                Logger.WriteLine($"Skipping user '{user.Username}' for role initialization because they are missing");
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
