using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flurl.Http;
using MahApps.Metro.Controls;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace RamMonitor.Services
{
    /// <summary>
    /// 某一个时间的RAM数据
    /// </summary>
    public class RAMDATA
    {
        public ulong BaseBalance { set; get; }

        public Double QuoteBalance { set; get; }

        public Double RAMPrice { set; get; }

        public DateTime time { set; get; }
    }

    /// <summary>
    /// RAM 行情
    /// </summary>
    public class RAMQUOTES
    {
        public RAMDATA LastData { set; get; }
        public string LastPercentage { set; get; }
        public string OneMinPercentage { set; get; }
        public string TenMinPercentage { set; get; }
        public string OneHourPercentage { set; get; }

    }

    public class RAM
    {

        public static List<RAMDATA> m_ramDataList;

        static RAM()
        {
            m_ramDataList = new List<RAMDATA>();
        }

        public static void Save()
        {
            //存储
        }


        public static void Load()
        {
            //载入
        }


        public static void GetSpeedServer()
        {
            //循环测试BP节点
        }


        /// <summary>
        /// 获取一次更新
        /// </summary>
        /// <returns>是否有变动</returns>
        public static bool UpdateData()
        {
            try
            {
                string json = GetTableJson("https://api.eosnewyork.io");
                if (json != string.Empty)
                {
                    JObject root = JObject.Parse(json);

                    //Debug.WriteLine(root.ToString());

                    var baseBalanceToken = root.SelectToken("$..base.balance");
                    var quoteBalanceToken = root.SelectToken("$..quote.balance");

                    //string baseBalance = Regex.Match(baseBalanceToken.ToString(), @"(?<RAM>\d+)", RegexOptions.IgnoreCase).Groups["RAM"].Value;
                    //string quoteBalance = Regex.Match(quoteBalanceToken.ToString(), @"(?<EOS>[0-9]*\.[0-9]*)", RegexOptions.IgnoreCase).Groups["EOS"].Value;
                    string baseBalance = baseBalanceToken.ToString().Split(" ".ToArray())[0];
                    string quoteBalance = quoteBalanceToken.ToString().Split(" ".ToArray())[0];

                    RAMDATA data = new RAMDATA();
                    data.BaseBalance = ulong.Parse(baseBalance);
                    data.QuoteBalance = Double.Parse(quoteBalance);
                    data.RAMPrice = data.QuoteBalance / data.BaseBalance * 1024;
                    data.time = DateTime.Now;


                    RAMDATA lastData = GetLastData();
                    if (lastData != null)
                    {
                        if (lastData.RAMPrice != data.RAMPrice)
                        {
                            Debug.WriteLine(data);
                            m_ramDataList.Add(data);
                            return true;
                        }
                    }
                    else
                    {
                        Debug.WriteLine(data);
                        m_ramDataList.Add(data);
                        return true;
                    }
                }
                return false;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        public static RAMDATA GetLastData()
        {
            var index = m_ramDataList.Count - 1;
            if (index >= 0 )
            {
                return m_ramDataList[index];
            }
            return null;
        }

        public static RAMDATA GetLastLastData()
        {
            var index = m_ramDataList.Count - 2;
            if (index >= 0)
            {
                return m_ramDataList[index];
            }
            return null;
        }

        public static RAMDATA GetFirstData()
        {
            if (m_ramDataList.Count >  0)
            {
                return m_ramDataList[0];
            }
            return null;
        }
        public static RAMQUOTES GetQuotes()
        {
            RAMQUOTES quotes = new RAMQUOTES();
            quotes.LastData = GetLastData();
            var lastLastData = GetLastLastData();

            if (lastLastData != null)
            {
                quotes.LastPercentage = GetProportion(quotes.LastData.RAMPrice, lastLastData.RAMPrice);
            }

            //计算一分钟的涨跌幅
            RAMDATA data = m_ramDataList.FindLast(a => Math.Abs((a.time - DateTime.Now).TotalSeconds) > 60 );
            if (data != null)
            {
                quotes.OneMinPercentage = GetProportion(quotes.LastData.RAMPrice, data.RAMPrice);
            }
            else
            {
                quotes.OneMinPercentage = GetProportion(quotes.LastData.RAMPrice, GetFirstData().RAMPrice);
            }

            data = m_ramDataList.FindLast(a => Math.Abs((a.time - DateTime.Now).TotalSeconds) > 60 * 10 );
            if (data != null)
            {
                quotes.TenMinPercentage = GetProportion(quotes.LastData.RAMPrice, data.RAMPrice);
            }
            else
            {
                quotes.TenMinPercentage = GetProportion(quotes.LastData.RAMPrice, GetFirstData().RAMPrice);
            }

            data = m_ramDataList.FindLast(a => Math.Abs((a.time - DateTime.Now).TotalSeconds) > 60 * 60 );
            if (data != null)
            {
                quotes.OneHourPercentage = GetProportion(quotes.LastData.RAMPrice, data.RAMPrice);
            }
            else
            {
                quotes.OneHourPercentage = GetProportion(quotes.LastData.RAMPrice, GetFirstData().RAMPrice);
            }

            return quotes;
        }

        


        public static string GetProportion(Double now, Double old)
        {
            string text = "";
            Double proportion = now / old;
            if (proportion > 1) //涨
            {
                proportion = (proportion - 1d) * 100;
                text = "+" + string.Format("{0:0.00}", proportion) + "%";
                Debug.WriteLine(now + "/" + old + "=" + text + " " + "(" + proportion + ")");
            }
            else if (proportion < 1d) //跌
            {
                proportion = (1d - proportion) * 100;
                text = "-" + string.Format("{0:0.00}", proportion) + "%";
                Debug.WriteLine(now + "/" + old + "=" + text +" " +  "(" + proportion + ")");
            }
            else //无变化
            {
                text = "+0.00%";
            }
            return text;
        }


        public static string GetTableJson(string url)
        {
            try
            {
	            string postData = JsonConvert.SerializeObject(new { json = "true", code = "eosio", scope = "eosio", table = "rammarket", limit = "10" });
                

                var fullUrl = url + "/v1/chain/get_table_rows";
                //Debug.WriteLine(fullUrl);
                var responseString = fullUrl.PostStringAsync(postData).Result;
                var json = responseString.Content.ReadAsStringAsync().Result;
	            return json;
            }
            catch (System.Exception ex)
            {
                //Debug.WriteLine(ex);
                return "";
            }
        }

    }
}
