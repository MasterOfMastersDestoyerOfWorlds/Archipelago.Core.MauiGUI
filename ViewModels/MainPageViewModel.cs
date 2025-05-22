using Archipelago.Core.MauiGUI.Logging;
using Archipelago.Core.MauiGUI.Models;
using Archipelago.Core.MauiGUI.Utils;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Serilog.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
        private ICommand _unstuckClickedCommand;
        private bool _unstuckVisible;
        private ItemsUpdatingScrollMode _autoscrollMode = ItemsUpdatingScrollMode.KeepLastItemInView;
        private bool _autoscrollEnabled = true;
        private readonly System.Timers.Timer _processingTimer;
        private readonly object _processingLock = new();
        private bool _isProcessingQueue = false;
        private const int MAX_BATCH_SIZE = 25; // Process messages in batches
        private const int TIMER_INTERVAL = 100; // Process queue every 100ms
        private readonly ConcurrentQueue<LogListItem> _messageQueue = new();

        public ObservableCollection<string> LogEventLevels { get; private set; } = Enum.GetNames(typeof(LogEventLevel)).ToObservableCollection();
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
        public ItemsUpdatingScrollMode AutoScrollMode
        {
            get
            {
                return _autoscrollMode;
            }
            set
            {
                if (_autoscrollMode != value)
                {
                    _autoscrollMode = value;
                    OnPropertyChanged();
                }
            }
        }
        public event EventHandler<ConnectClickedEventArgs> ConnectClicked;
        public event EventHandler<ArchipelagoCommandEventArgs> CommandReceived;
        public event EventHandler UnstuckClicked;
        public bool UnstuckVisible
        {
            get
            {
                return _unstuckVisible;
            }
            set
            {
                if (_unstuckVisible != value)
                {
                    _unstuckVisible = value; OnPropertyChanged();
                }
            }
        }
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
        public bool AutoscrollEnabled
        {
            get
            {
                return _autoscrollEnabled;
            }
            set
            {
                if (_autoscrollEnabled != value)
                {
                    _autoscrollEnabled = value;
                    OnPropertyChanged();

                    AutoScrollMode = _autoscrollEnabled ? ItemsUpdatingScrollMode.KeepLastItemInView : ItemsUpdatingScrollMode.KeepItemsInView;
                }
            }
        }
        public ICommand UnstuckClickedCommand
        {
            get
            {
                return _unstuckClickedCommand;
            }
            set
            {
                if (_unstuckClickedCommand != value)
                {
                    _unstuckClickedCommand = value; OnPropertyChanged();
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
        public MainPageViewModel(string archipelagoVersion = "0.5.1")
        {
            ConnectClickedCommand = new Command(() => { ConnectClicked?.Invoke(this, new ConnectClickedEventArgs { Host = Host, Slot = Slot, Password = Password }); });
            CommandSentCommand = new Command(() => { CommandReceived?.Invoke(this, new ArchipelagoCommandEventArgs { Command = CommandText }); CommandText = string.Empty; });
            UnstuckClickedCommand = new Command(() => { UnstuckClicked?.Invoke(this, EventArgs.Empty); });
            ClientVersion = Helpers.GetAppVersion();
            ArchipelagoVersion = archipelagoVersion;

            _processingTimer = new System.Timers.Timer(TIMER_INTERVAL);
            _processingTimer.Elapsed += ProcessMessageQueue;
            _processingTimer.AutoReset = true;
            _processingTimer.Start();

            LoggerConfig.Initialize((e, l) => WriteLine(e, l), (a, l) => WriteLine(a, l));
        }
        public void WriteLine(string output, LogEventLevel level)
        {
            _messageQueue.Enqueue(new LogListItem(output, GetColorForLogLevel(level)));
        }
        public void WriteLine(APMessageModel output, LogEventLevel level)
        {
            _messageQueue.Enqueue(new LogListItem(output));
        }

        private void ProcessMessageQueue(object sender, ElapsedEventArgs e)
        {
            // Prevent multiple concurrent processing
            if (_isProcessingQueue)
                return;


            lock (_processingLock)
            {
                try
                {
                    _isProcessingQueue = true;

                    List<LogListItem> textBatch = new();
                    int processedCount = 0;

                    while (_messageQueue.TryDequeue(out var item) && processedCount < MAX_BATCH_SIZE)
                    {
                        textBatch.Add(item);
                        processedCount++;
                    }

                    if (textBatch.Count > 0)
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            if (textBatch.Count > 0)
                            {
                                foreach (var item in textBatch)
                                {
                                    LogList.Add(item);
                                }
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error processing message queue: {ex.Message}");
                }
                finally
                {
                    _isProcessingQueue = false;
                }
            }
        }
        public override void Dispose()
        {
            _processingTimer?.Stop();
            _processingTimer?.Dispose();
            base.Dispose();
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
