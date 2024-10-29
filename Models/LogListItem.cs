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
        private ObservableCollection<Span> _textSpans = new ObservableCollection<Span>();

        public ObservableCollection<Span> TextSpans
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
            TextSpans = new ObservableCollection<Span>()
            {
                new Span(){Text = text},
            };
        }
        public LogListItem(IEnumerable<Span> spans)
        {
            TextSpans = spans.ToObservableCollection();
        }
        public LogListItem(APMessageModel message)
        {
            TextSpans = new ObservableCollection<Span>();
            foreach (var part in message.Parts)
            {
                var span = new Span();
                span.Text = part.Text;
                span.TextColor = Color.FromRgb(part.Color.R, part.Color.G, part.Color.B);
                TextSpans.Add(span);
            }
        }
    }
}
