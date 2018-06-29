using Flurl.Http;
using MahApps.Metro.Controls;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using RamMonitor.Services;
using System.ComponentModel;

namespace RamMonitor
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private bool runThread = true;


        public MainWindow()
        {
            InitializeComponent();

            //var ret =  Services.PowerShell.RunScriptText("docker -h");

            //HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\PowerShell\1\ShellIds\Microsoft.PowerShell\
            //ExecutionPolicy = RemoteSigned
            //Debug.WriteLine(ret.ResultText);
            //Services.RAM.UpdateData();
            //Services.SuperNode.GetSpeed();
            //test();
            var dp = DependencyPropertyDescriptor.FromProperty(
             TextBlock.TextProperty,
             typeof(TextBlock));

            dp.AddValueChanged(textLastPercentage, textBlockChangeText);
            dp.AddValueChanged(textOneMinPercentage, textBlockChangeText);
            dp.AddValueChanged(textTenMinPercentage, textBlockChangeText);
            dp.AddValueChanged(textOneHourPercentage, textBlockChangeText);

            Task.Run(() => { ActiveThread(); } );
        }

        private void textBlockChangeText(object sender, EventArgs e)
        {
            TextBlock text = (TextBlock)sender;
            if (text.Text.Contains("+"))
            {
                text.Foreground = new SolidColorBrush(Color.FromArgb(255, 243, 91, 139));
                
            }
            else if (text.Text.Contains("-"))
            {
                text.Foreground = new SolidColorBrush(Color.FromArgb(255, 155, 184, 58));
            }
        }


        ~MainWindow()
        {
            runThread = false;
        }

        /// <summary>
        /// 拖拽窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBlockMain_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void ActiveThread()
        {
            //每5秒一次
            while(runThread)
            {
                Services.RAM.UpdateData();
                var quotes = Services.RAM.GetQuotes();
                ShowQuotes(quotes);
                Thread.Sleep(1000 * 5);
                //刷新界面
            }
        }

        

        private void ShowQuotes(RAMQUOTES quotes)
        {
            if (quotes.LastData != null)
            {
                this.Dispatcher.Invoke(() => {
                    textNowAmount.Text = string.Format("{0:0.0000}", quotes.LastData.RAMPrice);

                    textLastPercentage.Text = string.IsNullOrWhiteSpace(quotes.LastPercentage) ? "*" : quotes.LastPercentage;
                    textOneMinPercentage.Text = string.IsNullOrWhiteSpace(quotes.OneMinPercentage) ? "*" : quotes.OneMinPercentage;
                    textTenMinPercentage.Text = string.IsNullOrWhiteSpace(quotes.TenMinPercentage) ? "*" : quotes.TenMinPercentage;
                    textOneHourPercentage.Text = string.IsNullOrWhiteSpace(quotes.OneHourPercentage) ? "*" : quotes.OneHourPercentage;

                    textNowAmount.Foreground = textLastPercentage.Foreground;

                });
            }

           
        }





    }
}
