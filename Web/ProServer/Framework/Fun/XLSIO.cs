using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using Syncfusion.XlsIO;
using System.IO;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Collections.Generic;

namespace ProServer
{
    /// <summary>
    /// Syncfusion.XlsIO无需安装excel
    /// </summary>
    public class XLSIO
    {
        /// <summary>
        /// 把DataTable生成，excel格式的byte[]
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static byte[] ExportExcel(DataTable dataTable, string fileName)
        {
            byte[] buffer;
            using (MemoryStream stream = new MemoryStream())
            {
                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    //IApplication application = excelEngine.Excel;
                    IWorkbook workbook = excelEngine.Excel.Workbooks.Create(1);
                    IWorksheet sheet = workbook.Worksheets[0];
                    sheet.Name = fileName;
                    sheet.ImportDataTable(dataTable, true, 1, 1);
                    sheet.UsedRange.AutofitColumns();

                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        if (sheet.Columns[i].ColumnWidth <= 252)
                            sheet.Columns[i].ColumnWidth += 3;
                        //sheet.Columns[i].NumberFormat = "General";
                    }

                    IRange range = sheet.Rows[0];
                    range.WrapText = true;
                    range.CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                    range.CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                    range.CellStyle.Font.Bold = true;

                    workbook.BuiltInDocumentProperties.Author = "easyman";
                    workbook.BuiltInDocumentProperties.Company = "联宇";

                    workbook.SaveAs(stream);

                    excelEngine.ThrowNotSavedOnDestroy = false;
                }

                stream.Position = 0;

                buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
            }

            return buffer;
        }
        /// <summary>
        /// 生成多个工作表,工作表名用逗号分开
        /// </summary>
        /// <param name="dataTableArr"></param>
        /// <param name="sheetNameArrStr"></param>
        /// <returns></returns>
        public static byte[] ExportExcel(DataTable[] dataTableArr, string sheetNameArrStr)
        {
            byte[] buffer;
            using (MemoryStream stream = new MemoryStream())
            {
                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    //IApplication application = excelEngine.Excel;
                    IWorkbook workbook = excelEngine.Excel.Workbooks.Create(dataTableArr.Length);
                    string[] nameArr = sheetNameArrStr.Split(',');
                    for (int a = 0; a < dataTableArr.Length; a++)
                    {
                        IWorksheet sheet = workbook.Worksheets[a];
                        if (nameArr.Length > a)
                            sheet.Name = nameArr[a];
                        else
                            sheet.Name = nameArr[nameArr.Length - 1];

                        sheet.ImportDataTable(dataTableArr[a], true, 1, 1);
                        sheet.UsedRange.AutofitColumns();
                        for (int i = 0; i < dataTableArr[a].Columns.Count; i++)
                        {
                            if (sheet.Columns[i].ColumnWidth <= 252)
                                sheet.Columns[i].ColumnWidth += 3;
                            //sheet.Columns[i].NumberFormat = "General";
                        }
                        IRange range = sheet.Rows[0];
                        range.WrapText = true;
                        range.CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                        range.CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                        range.CellStyle.Font.Bold = true;
                    }


                    workbook.BuiltInDocumentProperties.Author = "OUMA";
                    workbook.BuiltInDocumentProperties.Company = "欧码科技";

                    workbook.SaveAs(stream);

                    excelEngine.ThrowNotSavedOnDestroy = false;
                }
                stream.Position = 0;
                buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
            }
            return buffer;
        }


        /// <summary>
        /// 把DataTable转成xls文件
        /// </summary>
        /// <param name="savePath"></param>
        /// <param name="dataTableArr">生成多个工作表</param>
        /// <param name="sheetNameArrStr">工作表名用逗号分开</param>
        /// <returns></returns>
        public static string SaveXlsFile(string savePath, DataTable[] dataTableArr, string sheetNameArrStr)
        {
            byte[] inByte = ExportExcel(dataTableArr, sheetNameArrStr);
            FileStream fs = new FileStream(savePath, FileMode.Create, FileAccess.Write);
            fs.Write(inByte, 0, inByte.Length);
            fs.Close();
            return "";
        }

        /// <summary>
        /// 读取Excel
        /// </summary>
        /// <param name="path">地址</param>
        /// <param name="sheetNameArr">表名</param>
        /// <returns></returns>
        public static DataSet ReadExcel(string path, string[] sheetNameArr)
        {
            DataSet reDs = new DataSet();
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                //IApplication application = excelEngine.Excel;
                IWorkbook workbook = excelEngine.Excel.Workbooks.Open(path);
                for (int i = 0; i < sheetNameArr.Length; i++)//传入的制表名组
                {
                    for (int i1 = 0; i1 < workbook.Worksheets.Count; i1++)//所有的工作表
                    {
                        if (sheetNameArr[i] == workbook.Worksheets[i1].Name)
                        {
                            IWorksheet sheet = workbook.Worksheets[i1];
                            DataTable showDt = new DataTable();
                            for (int a = 0; a < sheet.Columns.Length; a++)
                            {
                                showDt.Columns.Add(sheet.Columns[a].DisplayText);
                            }
                            for (int a = 1; a < sheet.Rows.Length; a++)
                            {

                                string[] inStrArr = new string[sheet.Columns.Length];

                                for (int b = 0; b < sheet.Columns.Length; b++)
                                {
                                    try
                                    {
                                        inStrArr[b] = sheet.Rows[a].Cells[b].DisplayText;
                                    }
                                    catch
                                    {
                                        inStrArr[b] = null;
                                    }
                                }
                                showDt.Rows.Add(inStrArr);

                            }
                            reDs.Tables.Add(showDt);
                        }
                    }
                }
            }
            return reDs;
        }


        public static DataTable ReadExcelBySql(string path)
        {
            OleDbConnection con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1;\"");
            con.Open();
            DataSet ds = new DataSet();
            OleDbCommand cmd = new OleDbCommand("select * from [台账表$]", con);
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            da.Fill(ds);
            return ds.Tables[0];
        }

        public static DataTable ReadCsv(string path)
        {
            var rowsArr = File.ReadAllLines(path, System.Text.Encoding.Default);
            IList<IList<string>> columList = new List<IList<string>>();
            IList<IList<string>> columOld = new List<IList<string>>();
            for (int a = 0; a < 4; a++)
            {
                IList<string> row = new List<string>();
                var columArr = rowsArr[a].Split(',');
                columOld.Add(columArr);
                for (int b = 0; b < columArr.Count(); b++)
                {
                    string nowName = columArr[b];
                    if (string.IsNullOrEmpty(nowName))
                    {
                        if (a == 0)
                        {
                            nowName = row[b - 1];
                        }
                        else if (a == 1)
                        {
                            if (b > 6)
                            {
                                nowName = row[b - 1];
                            }
                            else
                            {
                                nowName = columList[0][b];
                            }
                        }
                        else if (a == 2)
                        {
                            if (b > 6)
                            {
                                if (columList[1][b].IndexOf("求和项") > 0)
                                {
                                    nowName = columList[1][b];
                                }
                                else
                                {
                                    nowName = row[b - 1];
                                }
                            }
                            else
                            {
                                nowName = columList[0][b];
                            }
                        }
                        else if (a == 3)
                        {
                            if (b > 6)
                            {
                                nowName = columOld[2][b];
                                if (string.IsNullOrEmpty(nowName))
                                {
                                    nowName = columOld[1][b];
                                    columList[2][b] = nowName;
                                }
                                if (string.IsNullOrEmpty(nowName))
                                {
                                    nowName = columOld[0][b];
                                    columList[2][b] = nowName;
                                    columList[1][b] = nowName;
                                }
                            }
                            else
                            {
                                nowName = columList[0][b];
                            }
                        }
                    }
                    row.Add(nowName);
                }
                columList.Add(row);
            }
            DataTable dt = new DataTable();

            for (int i = 0; i < columList[0].Count; i++)
            {
                string name = columList[0][i];
                for (int a = 1; a < 4; a++)
                {
                    if (columList[a][i] != columList[a - 1][i])
                    {
                        name += "," + columList[a][i];
                    }
                }
                dt.Columns.Add(name);
            }

            for (int i = 4; i < rowsArr.Count(); i++)
            {
                dt.Rows.Add(rowsArr[i].Split(','));
            }
            return dt;
        }


        /// <summary>
        /// 读取Excel,默认取第一个表
        /// </summary>
        /// <param name="path">地址</param>
        /// <returns></returns>
        public static DataTable ReadExcel(string path)
        {
            DataTable showDt = new DataTable();
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                //IApplication application = excelEngine.Excel;
                IWorkbook workbook = excelEngine.Excel.Workbooks.Open(path);

                if (workbook.Worksheets.Count > 0)//所有的工作表
                {
                    int i1 = 0;
                    IWorksheet sheet = workbook.Worksheets[i1];

                    for (int a = 0; a < sheet.Columns.Length; a++)
                    {
                        showDt.Columns.Add(sheet.Columns[a].DisplayText);
                    }
                    for (int a = 1; a < sheet.Rows.Length; a++)
                    {

                        string[] inStrArr = new string[sheet.Columns.Length];

                        for (int b = 0; b < sheet.Columns.Length; b++)
                        {
                            try
                            {
                                inStrArr[b] = sheet.Rows[a].Cells[b].DisplayText;
                            }
                            catch
                            {
                                inStrArr[b] = null;
                            }
                        }
                        showDt.Rows.Add(inStrArr);
                    }
                }
            }
            return showDt;
        }
    }
}

