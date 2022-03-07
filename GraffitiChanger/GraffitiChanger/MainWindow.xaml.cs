using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GraffitiChanger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow = this;
            stop_Button.IsEnabled = false;
            Terminal.labelOutput("##Program launched##");
        }
        public void window()
        {
        }

        private void start_Button_Click(object sender, RoutedEventArgs e)
        {
            Terminal.labelOutput("Process started");
            App.timer.Start();
            start_Button.IsEnabled = false;
            stop_Button.IsEnabled = true;
            App.isWorking = true;
        }
        private void stop_Button_Click(object sender, RoutedEventArgs e)
        {
            Terminal.labelOutput("Process stopped");
            App.timer.Stop();
            start_Button.IsEnabled = true;
            stop_Button.IsEnabled = false;
            App.isWorking = false;
        }
    }
}
