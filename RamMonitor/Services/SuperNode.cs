using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RamMonitor.Services
{
    public class SuperNodeData
    {
        public string Url { set; get; }
        public TimeSpan TimeSpan { set; get; }
    }

    public class SuperNode
    {

        public static List<SuperNodeData> superNodeDatas;

        public static void GetSpeed()
        {
            
            foreach(string url in Properties.Resources.BPList.Split("\r\n".ToArray()))
            {
                if (string.IsNullOrWhiteSpace(url) == false)
                {
                    //Debug.WriteLine(url);
                    CheckService(url);
                }

            }

        }


        public static SuperNodeData CheckService(string url)
        {
            DateTime beforDT = System.DateTime.Now;

            if (RAM.GetTableJson(url) != "")
            {
                DateTime afterDT = System.DateTime.Now;
                TimeSpan ts = afterDT.Subtract(beforDT);
                Console.WriteLine(url + " 总共花费{0}ms.", ts.TotalMilliseconds);
                SuperNodeData superNodeData = new SuperNodeData();
                superNodeData.TimeSpan = ts;
                superNodeData.Url = url;

                return superNodeData;
            }

            return null;
        }

    }
}
