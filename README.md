# Archipelago.Core.MauiGUI Documentation

A .NET MAUI user interface component for the Archipelago.Core library, providing a complete GUI solution for Archipelago game integrations with logging, messaging, item tracking, and connection management.

## Table of Contents
- [Installation](#installation)
- [Quick Start](#quick-start)
- [Features](#features)
- [Architecture](#architecture)
- [API Reference](#api-reference)
- [Customization](#customization)
- [Integration Examples](#integration-examples)
- [Troubleshooting](#troubleshooting)

## Installation

### Package Manager
```
Install-Package Archipelago.Core.MauiGUI
```

### .NET CLI
```
dotnet add package Archipelago.Core.MauiGUI
```

**Prerequisites:**
- Archipelago.Core package
- .NET MAUI application project
- Target platforms: Windows, Android, iOS, macOS

## Quick Start

### Basic Integration

```csharp
// In MauiProgram.cs
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // Register the MainPageViewModel
        builder.Services.AddSingleton<MainPageViewModel>();
        builder.Services.AddSingleton<MainPage>();

        return builder.Build();
    }
}

// In App.xaml.cs
public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();
    }
}
```

### Setting Up the Main Interface

```csharp
// In your main application class
public partial class MyArchipelagoApp : Application
{
    private ArchipelagoClient _archipelagoClient;
    private MainPageViewModel _viewModel;
    private MainPage _mainPage;

    public MyArchipelagoApp()
    {
        InitializeComponent();
        
        // Initialize the GUI components
        _viewModel = new MainPageViewModel("0.6.1"); // Archipelago version
        _mainPage = new MainPage(_viewModel);
        
        // Setup event handlers
        _viewModel.ConnectClicked += OnConnectClicked;
        _viewModel.CommandReceived += OnCommandReceived;
        _viewModel.UnstuckClicked += OnUnstuckClicked;
        
        MainPage = _mainPage;
    }

    private async void OnConnectClicked(object sender, ConnectClickedEventArgs e)
    {
        try
        {
            // Initialize your game client
            var gameClient = new GenericGameClient("YourGame.exe");
            _archipelagoClient = new ArchipelagoClient(gameClient);
            
            // Connect to Archipelago
            await _archipelagoClient.Connect(e.Host, "YourGameName");
            await _archipelagoClient.Login(e.Slot, e.Password);
            
            _viewModel.ConnectButtonEnabled = false;
            _viewModel.UnstuckVisible = true;
        }
        catch (Exception ex)
        {
            // Error handling - will appear in the log tab
            Log.Error($"Connection failed: {ex.Message}");
        }
    }

    private void OnCommandReceived(object sender, ArchipelagoCommandEventArgs e)
    {
        // Handle chat commands
        _archipelagoClient?.SendMessage(e.Command);
    }

    private void OnUnstuckClicked(object sender, EventArgs e)
    {
        // Handle unstuck functionality
        _archipelagoClient?.ForceReloadAllItems();
    }
}
```

## Features

### Multi-Tab Interface
The GUI provides three main tabs:

1. **Log Tab**: Real-time logging with color-coded messages
2. **Hints Tab**: Archipelago hint messages and player communications
3. **Received Items Tab**: Track all items received during the session

### Settings Flyout
- **Connection Settings**: Host, slot name, and password configuration
- **Log Level Control**: Adjustable logging verbosity (Error, Warning, Information, Debug, Verbose)
- **Auto-scroll Toggle**: Control automatic scrolling behavior
- **Version Information**: Display client and Archipelago versions
- **Unstuck Button**: Reset item processing when needed

### Advanced Logging
- **Color-coded Messages**: Different colors for log levels
- **Rich Text Support**: Multi-colored Archipelago messages with proper formatting
- **Performance Optimized**: Batched message processing to prevent UI freezing
- **Auto-scroll Management**: Smart scrolling that can be toggled on/off

## Architecture

### Key Components

```
Archipelago.Core.MauiGUI/
├── MainPage.xaml              # Main UI layout
├── MainPage.xaml.cs           # UI code-behind
├── ViewModels/
│   ├── MainPageViewModel.cs   # Main view model with business logic
│   └── ViewModelBase.cs       # Base class for MVVM pattern
├── Models/
│   ├── LogListItem.cs         # Log entry data model
│   ├── TextSpan.cs            # Rich text formatting
│   ├── ConnectClickedEventArgs.cs
│   └── ArchipelagoCommandEventArgs.cs
├── Logging/
│   ├── LoggerConfig.cs        # Serilog configuration
│   ├── ArchipelagoGuiSink.cs  # Custom Serilog sink
│   └── APMessageModel.cs      # Archipelago message models
└── Utils/
    ├── Extensions.cs          # Utility extensions
    └── Helpers.cs             # Helper methods
```

### MVVM Pattern
The GUI follows the Model-View-ViewModel pattern:
- **View**: XAML files define the UI structure
- **ViewModel**: Business logic and data binding
- **Model**: Data structures for logs, messages, and events

## API Reference

### MainPageViewModel

#### Properties
```csharp
// Connection Settings
string Host { get; set; }              // Archipelago server address
string Slot { get; set; }              // Player slot name
string Password { get; set; }          // Connection password
bool ConnectButtonEnabled { get; set; } // Enable/disable connect button

// Display Collections
ObservableCollection<LogListItem> LogList { get; }     // General log messages
ObservableCollection<LogListItem> HintList { get; }    // Hint messages
ObservableCollection<LogListItem> ItemList { get; }    // Received items

// UI Behavior
bool AutoscrollEnabled { get; set; }                   // Auto-scroll toggle
ItemsUpdatingScrollMode AutoScrollMode { get; }        // Scroll behavior
bool UnstuckVisible { get; set; }                      // Show unstuck button

// Version Information
string ClientVersion { get; }          // Application version
string ArchipelagoVersion { get; }     // Archipelago library version

// Logging
string SelectedLogLevel { get; set; }  // Current log level
ObservableCollection<string> LogEventLevels { get; }   // Available log levels

// Commands
string CommandText { get; set; }       // Text to send as command
```

#### Events
```csharp
event EventHandler<ConnectClickedEventArgs> ConnectClicked;   // Connection requested
event EventHandler<ArchipelagoCommandEventArgs> CommandReceived; // Chat command sent
event EventHandler UnstuckClicked;                           // Unstuck requested
```

#### Methods
```csharp
// Logging Methods
void WriteLine(string output, LogEventLevel level)           // Add text log entry
void WriteLine(APMessageModel output, LogEventLevel level)   // Add rich text entry

// Constructor
MainPageViewModel(string archipelagoVersion = "0.6.1")
```

### LogListItem

Represents a single log entry with rich text support:

```csharp
public class LogListItem : ViewModelBase
{
    ObservableCollection<TextSpan> TextSpans { get; set; }  // Rich text parts
    
    // Constructors
    LogListItem(string text)                                // Simple text
    LogListItem(string text, Color color)                   // Colored text
    LogListItem(IEnumerable<TextSpan> spans)                // Rich text spans
    LogListItem(APMessageModel message)                     // Archipelago message
}
```

### TextSpan

Individual text segment with formatting:

```csharp
public class TextSpan
{
    string Text { get; set; }        // Text content
    Color TextColor { get; set; }    // Text color
}
```

### Event Args Classes

```csharp
public class ConnectClickedEventArgs : EventArgs
{
    string Host { get; set; }        // Server address
    string Slot { get; set; }        // Player name
    string Password { get; set; }    // Connection password
}

public class ArchipelagoCommandEventArgs : EventArgs
{
    string Command { get; set; }     // Command text to send
}
```

## Customization

### Theming
The default theme uses a dark color scheme. Customize colors in MainPage.xaml:

```xml
<!-- Main background -->
<FlyoutPage BackgroundColor="#202020">
    
    <!-- Content areas -->
    <ContentPage BackgroundColor="#202020">
        
        <!-- Message frames -->
        <Frame BackgroundColor="#373737" />
        
        <!-- Input controls -->
        <Entry TextColor="Black" BackgroundColor="DarkGrey" />
        <Button BackgroundColor="DarkGrey" TextColor="Black" />
</FlyoutPage>
```

### Custom Log Colors
Modify log colors in MainPageViewModel:

```csharp
private Color GetColorForLogLevel(LogEventLevel level)
{
    return level switch
    {
        LogEventLevel.Error => Color.FromRgb(255, 100, 100),    // Light red
        LogEventLevel.Warning => Color.FromRgb(255, 255, 100),  // Light yellow
        LogEventLevel.Information => Color.FromRgb(200, 200, 200), // Light gray
        LogEventLevel.Debug => Color.FromRgb(150, 200, 255),    // Light blue
        LogEventLevel.Verbose => Color.FromRgb(180, 180, 180),  // Gray
        _ => Color.FromRgb(255, 255, 255)                       // White
    };
}
```

### Adding Custom Tabs
Extend the TabbedPage in MainPage.xaml:

```xml
<TabbedPage BackgroundColor="#202020">
    <!-- Existing tabs... -->
    
    <!-- Custom tab -->
    <ContentPage Title="My Custom Tab" BackgroundColor="#202020">
        <StackLayout>
            <Label Text="Custom Content" TextColor="White" />
            <!-- Your custom UI here -->
        </StackLayout>
    </ContentPage>
</TabbedPage>
```

### Custom Commands
Add command handling in your integration:

```csharp
private void OnCommandReceived(object sender, ArchipelagoCommandEventArgs e)
{
    var command = e.Command.ToLower();
    
    if (command.StartsWith("!help"))
    {
        _viewModel.WriteLine("Available commands: !help, !status, !reconnect", LogEventLevel.Information);
    }
    else if (command.StartsWith("!status"))
    {
        var status = _archipelagoClient.IsConnected ? "Connected" : "Disconnected";
        _viewModel.WriteLine($"Status: {status}", LogEventLevel.Information);
    }
    else if (command.StartsWith("!reconnect"))
    {
        // Implement reconnection logic
        Task.Run(async () => await ReconnectAsync());
    }
    else
    {
        // Send to Archipelago chat
        _archipelagoClient?.SendMessage(e.Command);
    }
}
```

## Integration Examples

### Complete Game Integration

```csharp
public partial class MyGameApp : Application
{
    private ArchipelagoClient _archipelagoClient;
    private MainPageViewModel _viewModel;
    private List<ILocation> _gameLocations;

    public MyGameApp()
    {
        InitializeComponent();
        
        // Setup GUI
        _viewModel = new MainPageViewModel("0.6.1");
        var mainPage = new MainPage(_viewModel);
        
        // Wire up events
        _viewModel.ConnectClicked += OnConnectClicked;
        _viewModel.CommandReceived += OnCommandReceived;
        _viewModel.UnstuckClicked += OnUnstuckClicked;
        
        MainPage = mainPage;
        
        // Load game locations
        _gameLocations = LoadGameLocations();
    }

    private async void OnConnectClicked(object sender, ConnectClickedEventArgs e)
    {
        try
        {
            _viewModel.ConnectButtonEnabled = false;
            _viewModel.WriteLine("Connecting to game...", LogEventLevel.Information);
            
            // Connect to game process
            var gameClient = new GenericGameClient("MyGame.exe");
            if (!gameClient.Connect())
            {
                throw new Exception("Could not connect to game process");
            }
            
            // Initialize Archipelago client
            _archipelagoClient = new ArchipelagoClient(gameClient);
            
            // Setup event handlers
            _archipelagoClient.ItemReceived += OnItemReceived;
            _archipelagoClient.LocationCompleted += OnLocationCompleted;
            _archipelagoClient.MessageReceived += OnMessageReceived;
            _archipelagoClient.Connected += OnArchipelagoConnected;
            _archipelagoClient.Disconnected += OnArchipelagoDisconnected;
            
            // Connect to Archipelago
            _viewModel.WriteLine($"Connecting to {e.Host}...", LogEventLevel.Information);
            await _archipelagoClient.Connect(e.Host, "My Game");
            
            _viewModel.WriteLine($"Logging in as {e.Slot}...", LogEventLevel.Information);
            await _archipelagoClient.Login(e.Slot, e.Password);
            
        }
        catch (Exception ex)
        {
            _viewModel.WriteLine($"Connection failed: {ex.Message}", LogEventLevel.Error);
            _viewModel.ConnectButtonEnabled = true;
        }
    }

    private async void OnArchipelagoConnected(object sender, ConnectionChangedEventArgs e)
    {
        _viewModel.WriteLine("Connected to Archipelago!", LogEventLevel.Information);
        _viewModel.UnstuckVisible = true;
        
        // Start monitoring locations
        await _archipelagoClient.MonitorLocations(_gameLocations);
        _viewModel.WriteLine($"Monitoring {_gameLocations.Count} locations", LogEventLevel.Information);
    }

    private void OnItemReceived(object sender, ItemReceivedEventArgs e)
    {
        _viewModel.WriteLine($"Received: {e.Item.Name}", LogEventLevel.Information);
        
        // Add to items tab
        _viewModel.ItemList.Add(new LogListItem($"[{DateTime.Now:HH:mm:ss}] {e.Item.Name}", 
            Color.FromRgb(100, 255, 100)));
        
        // Handle item in game
        HandleReceivedItem(e.Item);
    }

    private void OnLocationCompleted(object sender, LocationCompletedEventArgs e)
    {
        _viewModel.WriteLine($"Location found: {e.Location.Name}", LogEventLevel.Information);
    }

    private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        // Handle different message types
        if (e.Message is HintItemSendLogMessage hintMessage)
        {
            var hintText = $"[HINT] {hintMessage.Item} is at {hintMessage.Location}";
            _viewModel.HintList.Add(new LogListItem(hintText, Color.FromRgb(255, 255, 100)));
        }
        else if (e.Message is ChatLogMessage chatMessage)
        {
            var chatText = $"[{chatMessage.Player}] {chatMessage.Message}";
            _viewModel.WriteLine(chatText, LogEventLevel.Information);
        }
    }

    private void HandleReceivedItem(Item item)
    {
        // Implement game-specific item handling
        switch (item.Name)
        {
            case "Progressive Sword":
                UpgradePlayerSword();
                break;
            case "Magic Key":
                GivePlayerKey();
                break;
        }
    }

    private List<ILocation> LoadGameLocations()
    {
        // Load from JSON, database, or define programmatically
        return new List<ILocation>
        {
            new Location
            {
                Id = 1001,
                Name = "First Chest",
                Address = 0x800000,
                CheckType = LocationCheckType.Bit,
                AddressBit = 0,
                Category = "Chests"
            },
        };
    }
}
```

## Troubleshooting

### Common Issues

**UI Not Updating**
- Ensure you're using `MainThread.BeginInvokeOnMainThread()` for UI updates from background threads
- Check that ObservableCollection changes trigger PropertyChanged events

**Performance Issues with Large Log Files**
- The GUI automatically batches messages to prevent UI freezing

**Connection Settings Not Persisting**
- Implement settings persistence:

```csharp
// Save settings
Preferences.Set("archipelago_host", _viewModel.Host);
Preferences.Set("archipelago_slot", _viewModel.Slot);

// Load settings
_viewModel.Host = Preferences.Get("archipelago_host", "127.0.0.1:38281");
_viewModel.Slot = Preferences.Get("archipelago_slot", "Player1");
```

**Colors Not Displaying Correctly**
- Ensure you're using `Microsoft.Maui.Graphics.Color`, not `System.Drawing.Color`
- Check platform-specific color support

### Debugging Tips

**Enable Verbose Logging**
```csharp
_viewModel.SelectedLogLevel = "Verbose";
```

**Monitor Memory Usage**
```csharp
// Add to your main loop or timer
var memoryUsage = GC.GetTotalMemory(false) / 1024 / 1024;
_viewModel.WriteLine($"Memory usage: {memoryUsage} MB", LogEventLevel.Debug);
```

**Test UI Components**
```csharp
// Add test data to verify UI is working
_viewModel.LogList.Add(new LogListItem("Test log message", Color.FromRgb(255, 255, 255)));
_viewModel.ItemList.Add(new LogListItem("Test item", Color.FromRgb(100, 255, 100)));
_viewModel.HintList.Add(new LogListItem("Test hint", Color.FromRgb(255, 255, 100)));
```

## Performance Optimization

### Message Processing
The GUI includes built-in optimizations:
- Batched message processing (25 messages per batch)
- Timer-based queue processing (100ms intervals)
- Thread-safe message queuing

## License

This library is provided under the MIT License. See LICENSE file for details.

## Contributing

Contributions are welcome! Please submit issues and pull requests on the project repository.
