using System;
using System.IO;
using System.Windows;
using Tortoise;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Windows.Data;
using CommandLine;
using TortoiseDiscordBot.code.CommandLineOptions;

namespace TortoiseBotWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged; //INotifyPropertyChanged
        private object _syncLock = new object();

        private ObservableCollection<String> _consoleLog = new ObservableCollection<string>();
        private int consoleLogMaxSize = 20;

        private delegate void ButtonClickedCallbackDelegate();
        private ButtonClickedCallbackDelegate? _onBtnSetDefaultChannelDebugClicked;
        private ButtonClickedCallbackDelegate? _onBtnSetDefaultChannelLogClicked;
        private ButtonClickedCallbackDelegate? _onBtnTortoiseBotStart;
        private ButtonClickedCallbackDelegate? _onBtnTortoiseBotStop;

        private string _workingDirectory = "Not Set";
        private TortoiseBot? tortoiseBot;
        private Visibility _tortoiseBotGUIVisibility = Visibility.Hidden;

        public MainWindow()
        {
            DataContext = this;
            Closed += OnMainWindowClose;

            CommandLineOptionsManager.Initialize();
            CommandLineOptions options = CommandLineOptionsManager.GetOptions();
            if (!String.IsNullOrEmpty(options.WorkingDirectory))
            {
                WorkingDirectory = options.WorkingDirectory;
            }
            else
            {
                WorkingDirectory = Directory.GetCurrentDirectory();
            }

            InitializeComponent();
            BindingOperations.EnableCollectionSynchronization(_consoleLog, _syncLock);
            this.listConsoleLog.ItemsSource = _consoleLog;

            _onBtnTortoiseBotStart += StartTortoiseBot;
            _onBtnTortoiseBotStop += StopTortoiseBot;

            Logger.Initialize();
            Logger.AddOnLogLineCallbackAdded(OnLogLineAdded);
        }

        public void OnMainWindowClose(object? sender, EventArgs eventArgs)
        {
            _onBtnTortoiseBotStart -= StartTortoiseBot;
            _onBtnTortoiseBotStop -= StopTortoiseBot;

            StopTortoiseBot();
            Logger.RemoveOnLogLineCallbackAdded(OnLogLineAdded);
        }

        public void OnLogLineAdded(string logLine)
        {
            lock(_syncLock)
            {
                _consoleLog.Add(logLine);
                if (_consoleLog.Count > consoleLogMaxSize)
                {
                    _consoleLog.RemoveAt(0);
                }
            }
        }

        private void ClearLogs()
        {
            lock (_syncLock)
            {
                _consoleLog.Clear();
            }
        }

        public string WorkingDirectory
        {
            get
            {
                return _workingDirectory;
            }
            set
            {
                _workingDirectory = value;
                OnPropertyChanged("WorkingDirectory");
            }
        }
        
        public Visibility TortoiseBotGUIVisible
        {
            get
            {
                return _tortoiseBotGUIVisibility;
            }
            set
            {
                _tortoiseBotGUIVisibility = value;
                OnPropertyChanged("TortoiseBotGUIVisible");
            }
        }

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

        void StartTortoiseBot()
        {
            if(tortoiseBot is null)
            {
                BotStartupSettings botStartupSettings = new BotStartupSettings();
                string botStartupSettingsFile = $"{WorkingDirectory}\\TortoiseBotStartupSettings.json";
                if (!botStartupSettings.Initialize(botStartupSettingsFile))
                {
                    Logger.WriteLine($"Failed to parse BotStartupSettings. Cannot start bot.");
                    return;
                }

                BotRuntimeSettings botRuntimeSettings = new BotRuntimeSettings();
                string botSettingsFile = $"{WorkingDirectory}\\TortoiseBotRuntimeSettings.json";
                if (!botRuntimeSettings.ReadFromFile(botSettingsFile))
                {
                    Logger.WriteLine($"Failed to parse BotRuntimeSettings. Cannot start bot.");
                    return;
                }
                
                tortoiseBot = new TortoiseBot(botRuntimeSettings);
                _onBtnSetDefaultChannelDebugClicked += tortoiseBot.SetDefaultChannelToDebugChannel;
                _onBtnSetDefaultChannelLogClicked += tortoiseBot.SetDefaultChannelToLogChannel;
                tortoiseBot.Run(botStartupSettings);
                TortoiseBotGUIVisible = Visibility.Visible;
            }
            else
            {
                Logger.WriteLine("Cannot start TortoiseBot when they are already started");
            }
        }

        void StopTortoiseBot()
        {
            if(tortoiseBot != null)
            {
                TortoiseBotGUIVisible = Visibility.Hidden;
                _onBtnSetDefaultChannelDebugClicked -= tortoiseBot.SetDefaultChannelToDebugChannel;
                _onBtnSetDefaultChannelLogClicked -= tortoiseBot.SetDefaultChannelToLogChannel;
                tortoiseBot.SaveSettingsToDefaultFile();
                tortoiseBot = null;
                ClearLogs();
            }
            else
            {
                Logger.WriteLine("Cannot stop TortoiseBot when they are already stopped");
            }
        }

        void button_ClickSetDefaultChannelDebug(object sender, RoutedEventArgs eventArgs)
        {
            _onBtnSetDefaultChannelDebugClicked?.Invoke();
        }
        void button_ClickSetDefaultChannelLog(object sender, RoutedEventArgs eventArgs)
        {
            _onBtnSetDefaultChannelLogClicked?.Invoke();
        }
        void button_ClickTortoiseBotStop(object sender, RoutedEventArgs eventArgs)
        {
            _onBtnTortoiseBotStop?.Invoke();
        }
        void button_ClickTortoiseBotStart(object sender, RoutedEventArgs eventArgs)
        {
            _onBtnTortoiseBotStart?.Invoke();
        }
    }
}
