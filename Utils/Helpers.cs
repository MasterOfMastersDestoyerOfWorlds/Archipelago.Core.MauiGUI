using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archipelago.Core.MauiGUI.Utils
{
    public static class Helpers
    {
        public static string GetAppVersion()
        {
            var assembly = System.Reflection.Assembly.GetEntryAssembly();
            return assembly.GetName().Version.ToString();
        }
    }
}
