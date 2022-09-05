using OfficeOpenXml;

namespace SSC.Tools
{
    public static class ExcelExtension
    {
        public static byte[] CreateExcel<T>(List<T> elements, string[] columns, Func<T, object[]> RowAction, string worksheetName = "results")
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage())
            {
                package.Workbook.Worksheets.Add(worksheetName);
                ExcelWorksheet ws = package.Workbook.Worksheets[0];
                for (int i = 1; i <= columns.Length; i++)
                {
                    ws.Cells[1, i].Value = columns[i - 1];
                }

                int row = 2;
                foreach(var element in elements)
                {
                    var cells = RowAction(element);
                    int col = 1;
                    foreach(var cell in cells)
                    {
                        ws.Cells[row, col].Value = cell;
                        col++;
                    }
                    row++;
                }
                return package.GetAsByteArray();
            }
        }
    }
}
