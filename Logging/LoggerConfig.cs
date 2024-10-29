using Archipelago.MultiClient.Net.MessageLog.Messages;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archipelago.Core.MauiGUI.Logging
{
    public static class LoggerConfig
    {
        private static ILogger _logger;
        private static Action<string> _outputAction;
        private static Action<APMessageModel> _archipelagoEventLogHandler;

        public static void Initialize(Action<string> mainFormWriter,Action<APMessageModel> archipelagoEventLogHandler)
        {
            _outputAction = mainFormWriter;
            _archipelagoEventLogHandler = archipelagoEventLogHandler;
            var loggerConfiguration = new LoggerConfiguration()
                .WriteTo.ArchipelagoGuiSink(_outputAction, archipelagoEventLogHandler);

            _logger = loggerConfiguration.CreateLogger();
            Log.Logger = _logger;
        }
        public static void SetLogLevel(LogEventLevel level)
        {
            var loggerConfiguration = new LoggerConfiguration()
                .WriteTo.ArchipelagoGuiSink(_outputAction, _archipelagoEventLogHandler, level);
            _logger = loggerConfiguration.CreateLogger();
            Log.Logger = _logger;
        }
        public static LoggerConfiguration GetLoggerConfiguration(Action<string> mainFormWriter, Action<APMessageModel> archipelagoEventLogHandler)
        {
            return new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.ArchipelagoGuiSink(mainFormWriter, archipelagoEventLogHandler);
        }
    }
}
