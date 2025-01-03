using Archipelago.Core.MauiGUI.Logging;
using Archipelago.Core.MauiGUI.Utils;
using Archipelago.Core.MauiGUI.ViewModels;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = Microsoft.Maui.Graphics.Color;

namespace Archipelago.Core.MauiGUI.Models
{
    public class LogListItem : ViewModelBase
    {
        private ObservableCollection<TextSpan> _textSpans = new ObservableCollection<TextSpan>();

        public ObservableCollection<TextSpan> TextSpans
        {
            get { return _textSpans; }
            set
            {
                if (_textSpans != value)
                {
                    _textSpans = value;
                    OnPropertyChanged();
                }
            }
        }

        public LogListItem(string text)
        {
            TextSpans = new ObservableCollection<TextSpan>()
            {
                new TextSpan(){Text = text},
            };
        }
        public LogListItem(string text, Color color)
        {
            TextSpans = new ObservableCollection<TextSpan>()
            {
                new TextSpan(){Text = text, TextColor = color},
            };
        }
        public LogListItem(IEnumerable<TextSpan> spans)
        {
            TextSpans = spans.ToObservableCollection();
        }
        public LogListItem(IEnumerable<Span> spans)
        {
            var result =  new ObservableCollection<TextSpan>();
            foreach (var span in spans)
            {
                var textspan = new TextSpan();
                textspan.Text = span.Text;
                textspan.TextColor = span.TextColor;
                result.Add(textspan);
                result.Add(new TextSpan() { Text = " ", TextColor = Color.FromRgb(255, 255, 255) });
            }
            TextSpans = result;
        }
        public LogListItem(APMessageModel message)
        {
            TextSpans = new ObservableCollection<TextSpan>();
            foreach (var part in message.Parts)
            {
                var span = new TextSpan();
                span.Text = part.Text;
                span.TextColor = Color.FromRgb(part.Color.R, part.Color.G, part.Color.B);
                TextSpans.Add(span);
                TextSpans.Add(new TextSpan() { Text = " ", TextColor = Color.FromRgb(255,255,255)});
            }
        }
    }
}
