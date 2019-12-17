using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastNet.Framework.Npoi
{
    class ExcelUtils
    {
        public void Read(string filePath)
        {
            IWorkbook workbook = WorkbookFactory.Create(filePath);
            int sheetCount = workbook.NumberOfSheets;
            for (int sheetIndex = 0; sheetIndex < sheetCount; sheetIndex++)
            {
                ISheet sheet = workbook.GetSheetAt(sheetIndex);//获取第sheetIndex个工作表  
                if (sheet == null) 
                    continue;

                IRow row = sheet.GetRow(0);//获取第一行
                if (row == null) 
                    continue;

                int firstCellNum = row.FirstCellNum;
                int lastCellNum = row.LastCellNum;
                if (firstCellNum == lastCellNum) continue;
                string[] fields = row.Cells.Select(c => c.ToString()).ToArray();
                

                for (int i = 1; i <= sheet.LastRowNum; i++)//对工作表除去表头的每一行
                {
                    string cellValue = "";
                    for (int j = firstCellNum; j < lastCellNum; j++)
                    {
                        cellValue += sheet.GetRow(i).GetCell(j).ToString() + ",";//将每一行的数据以,相连
                    }
                }
            }
            workbook.Close();
        }
    }
}
