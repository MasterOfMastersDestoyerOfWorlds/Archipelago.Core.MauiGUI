using Archipelago.Core.MauiGUI.Logging;
using Archipelago.Core.MauiGUI.Models;
using Archipelago.Core.MauiGUI.Utils;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Color = Microsoft.Maui.Graphics.Color;

namespace Archipelago.Core.MauiGUI.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private ICommand _connectClickedCommand;
        private string _host;
        private string _slot;
        private string _password;
        private ObservableCollection<LogListItem> _logList = new ObservableCollection<LogListItem>();
        private string _clientVersion;
        private string _archipelagoVersion;
        private Microsoft.Maui.Graphics.Color _backgroundColor;
        private Color _textColor;
        private Color _buttonColor;
        private Color _buttonTextColor;
        private ICommand _commandSentCommand;
        private string _commandText;
        private string _selectedLogLevel;
        private bool _connectButtonEnabled;
        private ObservableCollection<LogListItem> _hintList = new ObservableCollection<LogListItem>();
        private ObservableCollection<LogListItem> _itemList = new ObservableCollection<LogListItem>();

        public ObservableCollection<string> LogEventLevels { get; private set; } =Enum.GetNames(typeof(LogEventLevel)).ToObservableCollection();
        public string SelectedLogLevel
        {
            get
            {
                var minLevel = LoggerConfig.GetMinimumLevel();
                return LogEventLevels.Single(x => x.ToLower() == LoggerConfig.GetMinimumLevel().ToString().ToLower());
            }
            set
            {
                if (_selectedLogLevel != value)
                {
                    LoggerConfig.SetLogLevel(Enum.Parse<LogEventLevel>(value));
                    OnPropertyChanged();
                }
            }
        }
        public event EventHandler<ConnectClickedEventArgs> ConnectClicked;
        public event EventHandler<ArchipelagoCommandEventArgs> CommandReceived;
        public bool ConnectButtonEnabled
        {
            get
            {
                return _connectButtonEnabled;
            }
            set
            {
                if (_connectButtonEnabled != value)
                {
                    _connectButtonEnabled = value; OnPropertyChanged();
                }
            }
        }
        public ICommand ConnectClickedCommand
        {
            get
            {
                return _connectClickedCommand;
            }
            set
            {
                if (_connectClickedCommand != value)
                {
                    _connectClickedCommand = value; OnPropertyChanged();
                }
            }
        }
        public ICommand CommandSentCommand
        {
            get
            {
                return _commandSentCommand;
            }
            set
            {
                if (_commandSentCommand != value)
                {
                    _commandSentCommand = value; OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<LogListItem> LogList
        {
            get { return _logList; }
            set
            {
                if (_logList != value)
                {
                    _logList = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<LogListItem> HintList
        {
            get { return _hintList; }
            set
            {
                if (_hintList != value)
                {
                    _hintList = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<LogListItem> ItemList
        {
            get { return _itemList; }
            set
            {
                if (_itemList != value)
                {
                    _itemList = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Host
        {
            get
            {
                return _host;
            }
            set
            {
                if (_host != value)
                {
                    _host = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Slot
        {
            get
            {
                return _slot;
            }
            set
            {
                if (_slot != value)
                {
                    _slot = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged();
                }
            }
        }
        public string CommandText
        {
            get
            {
                return _commandText;
            }
            set
            {
                if (_commandText != value)
                {
                    _commandText = value;
                    OnPropertyChanged();
                }
            }
        }
        public string ClientVersion
        {
            get
            {
                return _clientVersion;
            }
            set
            {
                if (_clientVersion != value)
                {
                    _clientVersion = value;
                    OnPropertyChanged();
                }
            }
        }
        public Color BackgroundColor
        {
            get
            {
                return _backgroundColor;
            }
            set
            {
                if (_backgroundColor != value)
                {
                    _backgroundColor = value;
                    OnPropertyChanged();
                }
            }
        }
        public Color TextColor
        {
            get
            {
                return _textColor;
            }
            set
            {
                if (_textColor != value)
                {
                    _textColor = value;
                    OnPropertyChanged();
                }
            }
        }
        public Color ButtonColor
        {
            get
            {
                return _buttonColor;
            }
            set
            {
                if (_buttonColor != value)
                {
                    _buttonColor = value;
                    OnPropertyChanged();
                }
            }
        }
        public Color ButtonTextColor
        {
            get
            {
                return _buttonTextColor;
            }
            set
            {
                if (_buttonTextColor != value)
                {
                    _buttonTextColor = value;
                    OnPropertyChanged();
                }
            }
        }
        public string ArchipelagoVersion
        {
            get
            {
                return _archipelagoVersion;
            }
            set
            {
                if (_archipelagoVersion != value)
                {
                    _archipelagoVersion = value;
                    OnPropertyChanged();
                }
            }
        }
        public MainPageViewModel(GuiDesignOptions options)
        {
            ConnectClickedCommand = new Command(() => { ConnectClicked?.Invoke(this, new ConnectClickedEventArgs { Host = Host, Slot = Slot, Password = Password }); });
            CommandSentCommand = new Command(() => { CommandReceived?.Invoke(this, new ArchipelagoCommandEventArgs { Command = CommandText }); CommandText = string.Empty; });
            ClientVersion = Helpers.GetAppVersion();
            ArchipelagoVersion = "0.5.0";

            if (options.BackgroundColor != null)
            {
                BackgroundColor = options.BackgroundColor;
            }
            if (options.ButtonColor != null)
            {
                ButtonColor = options.ButtonColor;
                ButtonColor = options.ButtonColor;
            }
            if (options.ButtonTextColor != null)
            {
                ButtonTextColor = options.ButtonTextColor;
                ButtonTextColor = options.ButtonTextColor;
            }
            if (options.TextColor != null)
            {
                TextColor = options.TextColor;
                TextColor = options.TextColor;
                TextColor = options.TextColor;
            }

            LoggerConfig.Initialize((e, l) => WriteLine(e, l), (a, l) => LogMessage(a, l));
        }
        public MainPageViewModel()
        {
            ConnectClickedCommand = new Command(() => { ConnectClicked?.Invoke(this, new ConnectClickedEventArgs { Host = Host, Slot = Slot, Password = Password }); });
            CommandSentCommand = new Command(() => { CommandReceived?.Invoke(this, new ArchipelagoCommandEventArgs { Command = CommandText }); CommandText = string.Empty; });
            ClientVersion = Helpers.GetAppVersion();
            ArchipelagoVersion = "0.5.0";

            LoggerConfig.Initialize((e, l) => WriteLine(e, l), (a, l) => LogMessage(a, l));
        }
        private void WriteLine(string output, LogEventLevel level)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                LogList.Add(new LogListItem(output, GetColorForLogLevel(level)));

            });

        }
        private void LogMessage(APMessageModel output, LogEventLevel level)
        {

            MainThread.BeginInvokeOnMainThread(() =>
            {
                LogList.Add(new LogListItem(output));
            });

        }
        private Color GetColorForLogLevel(LogEventLevel level)
        {
            Color logColor;
            switch (level)
            {
                case LogEventLevel.Error:
                    logColor = Color.FromRgb(255, 0, 0);
                    break;
                case LogEventLevel.Warning:
                    logColor = Color.FromRgb(255, 255, 0);
                    break;
                case LogEventLevel.Information:
                default:
                    logColor = Color.FromRgb(255, 255, 255);
                    break;
                case LogEventLevel.Debug:
                case LogEventLevel.Verbose:
                    logColor = Color.FromRgb(173, 216, 230);
                    break;
            }
            return logColor;
        }
    }
}
