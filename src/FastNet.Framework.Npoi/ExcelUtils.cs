using Newtonsoft.Json;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastNet.Framework.Npoi
{
    public class ExcelUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">序列化对象</typeparam>
        /// <param name="excelFilePath">xls文件路径</param>
        /// <param name="sheetIndex">工作表索引,默认0</param>
        /// <returns></returns>
        public static List<T> Read<T>(string excelFilePath, int sheetIndex = 0)
        {
            List<T> list = new List<T>();
            IWorkbook workbook = WorkbookFactory.Create(excelFilePath);
            //int sheetCount = workbook.NumberOfSheets;

            //获取第sheetIndex个工作表
            ISheet sheet = workbook.GetSheetAt(sheetIndex);
            if (sheet == null)
            {
                throw new Exception($"can not find sheet with index {sheetIndex}");
            }
            //获取第一行
            IRow row = sheet.GetRow(0);
            if (row == null)
            {
                throw new Exception($"no row with rownum 0");
            }

            int firstCellNum = row.FirstCellNum;
            int lastCellNum = row.LastCellNum;
            if (firstCellNum == lastCellNum)
            {
                throw new Exception($"no cell,firstCellNumm[{firstCellNum}],lastCellNum[{lastCellNum}]");
            }
            string[] fields = row.Cells.Select(c => c.ToString()).ToArray();

            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            //获取每一行(除去首行)
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                if (i > 1)
                    sb.Append(",");
                string cellValue = string.Empty;
                for (int j = firstCellNum; j < lastCellNum; j++)
                {
                    if (cellValue != string.Empty)
                        cellValue += ",";
                    cellValue += $"\"{fields[j].Trim()}\":\"{sheet.GetRow(i).GetCell(j).ToString()}\"";
                }
                sb.Append("{" + cellValue + "}");
            }
            sb.Append("]");
            list = JsonConvert.DeserializeObject<List<T>>(sb.ToString());
            workbook.Close();
            return list;
        }
    }
}
