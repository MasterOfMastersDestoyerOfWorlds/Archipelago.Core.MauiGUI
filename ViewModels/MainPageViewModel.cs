using Archipelago.Core.MauiGUI.Logging;
using Archipelago.Core.MauiGUI.Models;
using Archipelago.MultiClient.Net.MessageLog.Messages;
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

        public event EventHandler<ConnectClickedEventArgs> ConnectClicked;
        public event EventHandler<ArchipelagoCommandEventArgs> CommandReceived;
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
            ClientVersion = "0.0.1";
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

            LoggerConfig.Initialize((e) => WriteLine(e), (a)=> LogMessage(a));
        }
        public MainPageViewModel()
        {
            ConnectClickedCommand = new Command(() => { ConnectClicked?.Invoke(this, new ConnectClickedEventArgs { Host = Host, Slot = Slot, Password = Password }); });
            CommandSentCommand = new Command(() => { CommandReceived?.Invoke(this, new ArchipelagoCommandEventArgs { Command = CommandText }); CommandText = string.Empty; });
            ClientVersion = "0.0.1";
            ArchipelagoVersion = "0.5.0";

            LoggerConfig.Initialize((e) => WriteLine(e), (a) => LogMessage(a));
        }
        private void WriteLine(string output)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                LogList.Add(new LogListItem(output));
                
            });
            
        }
        private void LogMessage(APMessageModel output)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                LogList.Add(new LogListItem(output));
            });

        }
    }
}
