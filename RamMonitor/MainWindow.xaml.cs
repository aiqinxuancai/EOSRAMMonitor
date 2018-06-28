using Flurl.Http;
using MahApps.Metro.Controls;
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

namespace RamMonitor
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            var ret =  Services.PowerShell.RunScriptText("docker -h");
            //
            //HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\PowerShell\1\ShellIds\Microsoft.PowerShell\
            //ExecutionPolicy = RemoteSigned

            test();
        }

        public void test()
        {


            //var responseString = await "https://api.eosnewyork.io/v1/chain/get_table_rows"
            //.PostUrlEncodedAsync(new { json = "true", code = "eosio", scope = "eosio", table = "global" })
            //.ReceiveString();

            //HttpClient client = new HttpClient();
            //var values = new Dictionary<string, string>
            //{
            //   { "json", "true" },
            //   { "code", "eosio" },
            //    { "scope", "eosio" },
            //   { "table", "global" }
            //};
            //client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36");
            //var content = new FormUrlEncodedContent(values);

            //var response = await client.PostAsync("https://api.eosnewyork.io/v1/chain/get_table_rows", content);

            //var responseString2 = await response.Content.ReadAsStringAsync();



            var data = "https://www.feexplorer.io/EOS_RAM_price".GetStringAsync().Result;

            /*
                <h6>Current <span style="color:#734ce3;">RAM</span> Price</h6>
                <p style="font-size:120%;">0.11985765 EOS per kb</p>
                </div>
            */




            //Regex reg = new Regex(@"RAM</span> Price</h6>\r\n<p style=""font-size:120%;"">([0-9]*\.[0-9]*) EOS per");
            Regex reg = new Regex(@"([0-9]*\.[0-9]*) EOS per");
            var match = reg.Match(data);
            Debug.WriteLine(data);
            var groups = match.Groups;
            foreach (Group item in groups)
            {
                Debug.WriteLine(item.ToString());
            }

           



        }



    }
}
