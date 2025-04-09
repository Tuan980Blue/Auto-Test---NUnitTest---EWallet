using OfficeOpenXml;

namespace Test_EWallet.Test_NapTien
{
    public class ExcelReader
    {
        public static IEnumerable<object[]> ReadExcel(string filePath)
        {
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    var amount = worksheet.Cells[row, 1].Text;
                    var selectPaymentMethod = worksheet.Cells[row, 2].Text;
                    var expectedResult = worksheet.Cells[row, 3].Text;

                    yield return new object[] { amount, selectPaymentMethod, expectedResult };
                }
            }
        }
    }
}