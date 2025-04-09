namespace Test_EWallet.Lib_Excel;

public class ExcelDataProvider
{
    //doc du lieu tu file excel
    public static IEnumerable<object[]> ReadExcel(string filePath)
    {
        return ExcelReader.ReadExcel(filePath);
    }

    //ghi ket qua test vao file excel
    public static void WriteTestResult(string filePath, string num1, string num2, string expected, string actual, string status)
    {
        ExcelWriter.WriteTestResult(filePath, num1, num2, expected, actual, status);
    }

    //kiem tra file co dang duoc su dung hay khong
    public static bool IsFileInUse(string filePath)
    {
        return FileUtil.IsFileInUse(filePath);
    }
}