using OfficeOpenXml;
using System.Data;

namespace Test_EWallet.Test_NapTien
{
    public class ExcelDataProvider
    {
        public static IEnumerable<object[]> ReadExcel(string filePath)
        {
            return ExcelReader.ReadExcel(filePath);
        }

        public static void WriteTestResult(string filePath, string amount, string selectPaymentMethod, string expected, string actual, string status)
        {
            ExcelWriter.WriteTestResult(filePath, amount, selectPaymentMethod, expected, actual, status);
        }

        public static bool IsFileInUse(string filePath)
        {
            return FileUtil.IsFileInUse(filePath);
        }
    }
}