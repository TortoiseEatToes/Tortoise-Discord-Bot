using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tortoise
{
    public abstract class DiscordBot
    {
        protected DiscordSocketClient m_DiscordSocketClient;
        protected CommandService m_CommandService;

        public DiscordBot()
        {
            m_DiscordSocketClient = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                GatewayIntents = GatewayIntents.All,
                AlwaysDownloadUsers = true,
                UseInteractionSnowflakeDate = false,
                ConnectionTimeout = 60000
            });

            m_CommandService = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = false,
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Critical
            });

            m_DiscordSocketClient.Ready += OnClientIsReady;
            m_DiscordSocketClient.Log += OnClientLog;
            m_DiscordSocketClient.MessageReceived += OnMessageReceived;
            m_DiscordSocketClient.SlashCommandExecuted += OnSlashCommandCalled;
            m_DiscordSocketClient.UserJoined += OnUserJoined;
            m_DiscordSocketClient.UserLeft += OnUserLeft;
            m_DiscordSocketClient.UserVoiceStateUpdated += OnUserVoiceStateUpdated;
            m_DiscordSocketClient.ButtonExecuted += OnButtonClicked;
            m_DiscordSocketClient.JoinedGuild += OnJoinedGuild;
            m_DiscordSocketClient.ReactionAdded += OnReactionAdded;
            m_DiscordSocketClient.ReactionRemoved += OnReactionRemoved;
            m_DiscordSocketClient.PresenceUpdated += PresenceUpdated;
        }

        protected virtual async Task PresenceUpdated(SocketUser socketUser, SocketPresence socketPresence1, SocketPresence socketPresence2)
        {
            Logger.WriteLine("PresenceUpdated");
        }

        protected virtual async Task OnReactionRemoved(Cacheable<IUserMessage, ulong> arg1, Cacheable<IMessageChannel, ulong> arg2, SocketReaction arg3)
        {
            Logger.WriteLine("OnReactionRemoved");
        }

        protected virtual async Task OnReactionAdded(Cacheable<IUserMessage, ulong> arg1, Cacheable<IMessageChannel, ulong> arg2, SocketReaction arg3)
        {
            Logger.WriteLine("OnReactionAdded");
        }

        public async Task Run(string token)
        {
            await m_DiscordSocketClient.LoginAsync(TokenType.Bot, token);
            await m_DiscordSocketClient.StartAsync();

            Assembly? assembly = Assembly.GetEntryAssembly();
            if (assembly == null)
            {
                assembly = Assembly.GetCallingAssembly();
            }
            IServiceProvider? serviceProvider = null;
            await m_CommandService.AddModulesAsync(assembly, serviceProvider);
        }

        protected virtual async Task OnClientIsReady()
        {
            Logger.WriteLine("OnClientIsReady - Start");
        }

        private async Task OnClientLog(LogMessage logMessage)
        {
            Logger.WriteLineForwardedLog(logMessage.ToString());
        }

        protected virtual async Task OnMessageReceived(SocketMessage socketMessage)
        {
            Logger.WriteLine("OnMessageReceived: " + socketMessage.ToString());
        }

        protected virtual async Task OnSlashCommandCalled(SocketSlashCommand socketSlashCommand)
        {
            Logger.WriteLine("OnSlashCommandCalled: " + socketSlashCommand.ToString());
        }

        protected virtual async Task OnUserJoined(SocketGuildUser socketGuildUser)
        {
            Logger.WriteLine("OnUserJoined: " + socketGuildUser.ToString());
        }

        protected virtual async Task OnUserLeft(SocketGuild socketGuild, SocketUser socketUser)
        {
            Logger.WriteLine("OnUserLeft.socketGuild: " + socketGuild.ToString());
            Logger.WriteLine("OnUserLeft.socketUser: " + socketUser.ToString());
        }

        protected virtual async Task OnUserVoiceStateUpdated(SocketUser socketUser, SocketVoiceState socketVoiceState1, SocketVoiceState socketVoiceState2)
        {
            Logger.WriteLine("OnUserVoiceStateUpdated.socketUser: " + socketUser.ToString());
            Logger.WriteLine("OnUserVoiceStateUpdated.socketVoiceState1: " + socketVoiceState1.ToString());
            Logger.WriteLine("OnUserVoiceStateUpdated.socketVoiceState2: " + socketVoiceState2.ToString());
        }

        protected virtual async Task OnButtonClicked(SocketMessageComponent socketMessageComponent)
        {
            Logger.WriteLine("OnButtonClicked: " + socketMessageComponent.ToString());
        }

        protected virtual async Task OnJoinedGuild(SocketGuild socketGuild)
        {
            Logger.WriteLine("OnJoinedGuild: " + socketGuild.ToString());
        }

        public DiscordSocketClient GetSocketClient()
        {
            return m_DiscordSocketClient;
        }

        public bool GuildUserIsSelf(IGuildUser? guildUser)
        {
            return m_DiscordSocketClient.CurrentUser.Id == guildUser?.Id;
        }
    }
}
