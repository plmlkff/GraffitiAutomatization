using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Windows.Threading;
using System.IO;

namespace GraffitiChanger
{
    public partial class App : Application
    {
        public static DispatcherTimer timer = new DispatcherTimer();
        public static bool isWorking = false;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ExcelLogic._getIpAndPassList();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 10);
            Thread timeThr = new Thread(TimeLogic._timeThrLogic);
            timeThr.Start();
        }


        private void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                timer.Stop();
                Thread backendThread = new Thread(new ThreadStart(GraffitiCheck._startChecking));
                backendThread.Start();
            }
            catch (Exception ex)
            {
                Terminal.labelOutput(ex.Message);
            }
        }


    }
    class Terminal
    {
        public async static void labelOutput(string text)
        {
            string pathOfLogFile = Environment.CurrentDirectory + @"\Logs\" + DateTime.Today.ToShortDateString().ToString() + ".txt";

            if (!File.Exists(pathOfLogFile))//Checking if a log file exists
            {
                File.Create(pathOfLogFile).Close();//A log file is created if it has not already been created
            }
            await Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                MainWindow main = (MainWindow)Application.Current.MainWindow;

                string message = DateTime.Now.ToLongTimeString() + $":    {text}\r\n";

                main.label_Terminal.Text += message;
                File.AppendAllText(pathOfLogFile, message);

            }));
        }
    }
}
