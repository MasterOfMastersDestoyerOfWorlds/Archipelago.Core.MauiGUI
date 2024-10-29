using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archipelago.Core.MauiGUI.Models
{
    public class GuiDesignOptions
    {
        public string Title { get; set; }
        public Color? BackgroundColor { get; set; } = null;
        public Color? ButtonColor { get; set; } = null;
        public Color? ButtonTextColor { get; set; } = null;
        public Color? TextColor { get; set; } = null;
    }
}
