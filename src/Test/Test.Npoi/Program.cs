
using FastNet.Framework.Npoi;
using Newtonsoft.Json;
using System;

namespace Test.Npoi
{
    class Program
    {
        static void Main(string[] args)
        {
            var list = ExcelUtils.Read<TestInfo>("C:\\Users\\XUNZHI\\Desktop\\主板A股.xlsx");
        }
    }

    public class TestInfo
    {
        [JsonProperty("公司代码")]
        public string CompanyCode { get; set; }
        [JsonProperty("公司简称")]
        public string CompanyName { get; set; }
        [JsonProperty("代码")]
        public string StockCode { get; set; }
        [JsonProperty("简称")]
        public string StockName { get; set; }
        [JsonProperty("上市日期")]
        public string ListingDate { get; set; }
    }
}
