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
            m_DiscordSocketClient.InviteCreated += InviteCreated;
            m_DiscordSocketClient.InviteDeleted += InviteDeleted;
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

        public DiscordSocketClient GetSocketClient()
        {
            return m_DiscordSocketClient;
        }

        public bool GuildUserIsSelf(IGuildUser? guildUser)
        {
            return m_DiscordSocketClient.CurrentUser.Id == guildUser?.Id;
        }

        #region DiscordOverrides

        protected virtual Task OnClientIsReady()
        {
            Logger.WriteLine_Debug("OnClientIsReady - Start");
            return Task.CompletedTask;
        }

        private Task OnClientLog(LogMessage logMessage)
        {
            switch (logMessage.Severity)
            {
                case LogSeverity.Critical:
                    Logger.WriteLine_Critical(logMessage.Message);
                    break;
                case LogSeverity.Error:
                    Logger.WriteLine_Error(logMessage.Message);
                    break;
                case LogSeverity.Warning:
                    Logger.WriteLine_Warning(logMessage.Message);
                    break;
                case LogSeverity.Info:
                    Logger.WriteLine_Info(logMessage.Message);
                    break;
                case LogSeverity.Verbose:
                    Logger.WriteLine_Verbose(logMessage.Message);
                    break;
                case LogSeverity.Debug:
                    Logger.WriteLine_Debug(logMessage.Message);
                    break;
                default:
                    Logger.WriteLine_Debug("UNRECOGNIZED SEVERITY " + logMessage.Message);
                    break;
            }
            return Task.CompletedTask;
        }

        protected virtual Task PresenceUpdated(SocketUser socketUser, SocketPresence socketPresence1, SocketPresence socketPresence2)
        {
            Logger.WriteLine_Debug("PresenceUpdated");
            return Task.CompletedTask;
        }

        protected virtual Task OnReactionRemoved(Cacheable<IUserMessage, ulong> arg1, Cacheable<IMessageChannel, ulong> arg2, SocketReaction arg3)
        {
            Logger.WriteLine_Debug("OnReactionRemoved");
            return Task.CompletedTask;
        }

        protected virtual Task OnReactionAdded(Cacheable<IUserMessage, ulong> arg1, Cacheable<IMessageChannel, ulong> arg2, SocketReaction arg3)
        {
            Logger.WriteLine_Debug("OnReactionAdded");
            return Task.CompletedTask;
        }

        protected virtual Task OnMessageReceived(SocketMessage socketMessage)
        {
            Logger.WriteLine_Debug("OnMessageReceived: " + socketMessage.ToString());
            return Task.CompletedTask;
        }

        protected virtual Task OnSlashCommandCalled(SocketSlashCommand socketSlashCommand)
        {
            Logger.WriteLine_Debug("OnSlashCommandCalled: " + socketSlashCommand.ToString());
            return Task.CompletedTask;
        }

        protected virtual Task OnUserJoined(SocketGuildUser socketGuildUser)
        {
            Logger.WriteLine_Debug("OnUserJoined: " + socketGuildUser.ToString());
            return Task.CompletedTask;
        }

        protected virtual Task OnUserLeft(SocketGuild socketGuild, SocketUser socketUser)
        {
            Logger.WriteLine_Debug("OnUserLeft.socketGuild: " + socketGuild.ToString());
            Logger.WriteLine_Debug("OnUserLeft.socketUser: " + socketUser.ToString());
            return Task.CompletedTask;
        }

        protected virtual Task OnUserVoiceStateUpdated(SocketUser socketUser, SocketVoiceState socketVoiceState1, SocketVoiceState socketVoiceState2)
        {
            Logger.WriteLine_Debug("OnUserVoiceStateUpdated.socketUser: " + socketUser.ToString());
            Logger.WriteLine_Debug("OnUserVoiceStateUpdated.socketVoiceState1: " + socketVoiceState1.ToString());
            Logger.WriteLine_Debug("OnUserVoiceStateUpdated.socketVoiceState2: " + socketVoiceState2.ToString());
            return Task.CompletedTask;
        }

        protected virtual Task OnButtonClicked(SocketMessageComponent socketMessageComponent)
        {
            Logger.WriteLine_Debug("OnButtonClicked: " + socketMessageComponent.ToString());
            return Task.CompletedTask;
        }

        protected virtual Task OnJoinedGuild(SocketGuild socketGuild)
        {
            Logger.WriteLine_Debug("OnJoinedGuild: " + socketGuild.ToString());
            return Task.CompletedTask;
        }

        protected virtual Task InviteCreated(SocketInvite socketInvite)
        {
            Logger.WriteLine_Debug("InviteCreated: " + socketInvite.ToString());
            return Task.CompletedTask;
        }

        public virtual Task InviteDeleted(SocketGuildChannel socketGuildChannel, string name)
        {
            Logger.WriteLine_Debug("InviteDeleted: " + socketGuildChannel.ToString());
            return Task.CompletedTask;
        }

        #endregion
    }
}
