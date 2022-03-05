using Microsoft.Win32;
using System;
using System.Linq;
using System.IO;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows;

namespace GraffitiChanger
{
    class ExcelLogic
    {
        static int lastColumn = 0;
        static int lastRow = 0;
        public static string[][] data = new string[20][];
        public static void _getIpAndPassList()
        {
            if (File.Exists(Path.Combine(Environment.CurrentDirectory, "data.xlsx")))
            {
                Excel.Application application = new Excel.Application();
                Excel.Workbook workbook = application.Workbooks.Open(Path.Combine(Environment.CurrentDirectory, "data.xlsx"));
                Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Sheets[1];
                var lastCell = worksheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell);
                lastColumn = lastCell.Column;
                lastRow = lastCell.Row;
                for (int i = 0; i < lastRow; i++)
                {
                    data[i] = new string[3];
                    for (int j = 0; j < lastColumn; j++)
                    {
                        data[i][j] = (worksheet.Cells[i + 1, j + 1] as Excel.Range).Value2?.ToString();
                    }
                }
                workbook.Close();
                application.Quit();
                GC.Collect();
            }
            else
            {
                MessageBox.Show("There is no database with information about servers. Check the current directory and restart the program", "Error");
                App.labelOutput("There is no database with information about servers. Check current directory");
                throw new Exception();
            }
        }
        public static void _commitingChanges(string oldGraff, string newGraff)
        {
            Excel.Application application = new Excel.Application();
            Excel.Workbook workbook = application.Workbooks.Open(Path.Combine(Environment.CurrentDirectory, "data.xlsx"));
            Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Sheets[1];
            var lastCell = worksheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell);
            lastColumn = lastCell.Column;
            lastRow = lastCell.Row;
            for (int i = 0; i < lastRow; i++)
            {
                worksheet.Cells[i + 1, 3] = data[i][2];
            }
            workbook.Save();
            workbook.Close();
            application.Quit();
        }
    }
}
