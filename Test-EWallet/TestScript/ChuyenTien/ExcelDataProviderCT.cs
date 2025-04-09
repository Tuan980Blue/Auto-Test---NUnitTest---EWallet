namespace Test_EWallet.Test_NapTien
{
    public class ExcelDataProviderCT
    {
        public static IEnumerable<object[]> ReadExcel(string filePath)
        {
            return ExcelReaderCT.ReadExcel(filePath);
        }

        public static void WriteTestResult(string filePath, string num1, string expected, string actual, string status)
        {
            ExcelWriterCT.WriteTestResult(filePath, num1, expected, actual, status);
        }

        public static bool IsFileInUse(string filePath)
        {
            return FileUtil.IsFileInUse(filePath);
        }
    }
}