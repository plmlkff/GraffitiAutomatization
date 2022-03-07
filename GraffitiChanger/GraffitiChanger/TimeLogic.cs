using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Timers;

namespace GraffitiChanger
{
    class TimeLogic
    {
        public static Timer timerFrame = new Timer(2000);
        public static void _timeThrLogic()
        {
            timerFrame.Elapsed += _changeFrame;
            timerFrame.Interval = 1000;
            timerFrame.AutoReset = true;
            timerFrame.Start();
        }

        async static void _changeFrame(object sender, ElapsedEventArgs e)
        {
            await Application.Current.Dispatcher.BeginInvoke(new Action(()=>
            {
                MainWindow main = (MainWindow)Application.Current.MainWindow;
                main.label_Time.Content = DateTime.Now.ToLongTimeString();
            }));
        }
    }
}
