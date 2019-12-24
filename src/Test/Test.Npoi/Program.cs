
using FastNet.Framework.Npoi;
using System;

namespace Test.Npoi
{
    class Program
    {
        static void Main(string[] args)
        {
            var list = ExcelUtils.Read<TestInfo>("C:\\Users\\XUNZHI\\Desktop\\近半年专栏资讯统计.xlsx");
        }
    }

    public class TestInfo
    {
        public int ColumnID { get; set; }
        public string ColumnName { get; set; }
        public int NewsCount { get; set; }
    }
}
