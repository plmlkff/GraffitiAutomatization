using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace GraffitiChanger
{
    public partial class App : Application
    {
        public static DispatcherTimer timer = new DispatcherTimer();
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ExcelLogic._getIpAndPassList();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, 15, 0);
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            GraffitiCheck._startChecking();
        }

        public static void labelOutput(string text)
        {
            string message = DateTime.Now.ToLongTimeString() + $":    {text}\r\n";
        }
    }
}
