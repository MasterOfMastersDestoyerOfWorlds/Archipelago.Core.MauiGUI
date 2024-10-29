using Serilog.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Newtonsoft.Json;
using Windows.UI.WebUI;

namespace Archipelago.Core.MauiGUI.Logging
{
    public class ArchipelagoGuiSink : ILogEventSink
    {
        private LogEventLevel _logLevel;
        private Action<string> _outputEvent;
        private Action<APMessageModel> _archipelagoEventLogHandler;
        public ArchipelagoGuiSink(Action<string> outputEvent, Action<APMessageModel> archipelagoEventLogHandler, LogEventLevel level = LogEventLevel.Information)
        {
            _logLevel = level;
            _outputEvent = outputEvent;
            _archipelagoEventLogHandler = archipelagoEventLogHandler;
        }
        public void Emit(LogEvent logEvent)
        {
            try
            {                
                var logMessage = JsonConvert.DeserializeObject<APMessageModel>(logEvent.MessageTemplate.Text);
                

                _archipelagoEventLogHandler?.Invoke(logMessage);
                return;
            }
            catch (Exception ex)
            {
            }//not a json
            if(logEvent.Level <= _logLevel)
            {
                _outputEvent?.Invoke(logEvent.RenderMessage());
            }
        }
    }
    public static class ArchipelagoGuiSinkExtensions
    {
        public static LoggerConfiguration ArchipelagoGuiSink(
                  this LoggerSinkConfiguration loggerConfiguration, Action<string> outputEvent, Action<APMessageModel> archipelagoEventLogHandler, LogEventLevel level = LogEventLevel.Information)
        {
            return loggerConfiguration.Sink(new ArchipelagoGuiSink(outputEvent, archipelagoEventLogHandler, level));
        }
    }
}
