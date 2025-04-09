using System.Data;
using ExcelDataReader;

namespace Test_EWallet.Lib_Excel;

public class ExcelReader
{
    public static IEnumerable<object[]> ReadExcel(string filePath)
    {
        // 🔥 FIX: Đăng ký provider hỗ trợ encoding 1252 (giải quyết lỗi NotSupportedException)
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"⚠️ File Excel không tồn tại: {filePath}");
        }

        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            using (var excelReader = ExcelReaderFactory.CreateReader(stream))
            {
                var dataSet = excelReader.AsDataSet(new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = true // Đọc dòng đầu tiên làm tiêu đề cột
                    }
                });

                // Kiểm tra nếu không có dữ liệu
                if (dataSet.Tables.Count == 0)
                {
                    throw new Exception("⚠️ File Excel không chứa bảng dữ liệu nào.");
                }

                DataTable table = dataSet.Tables[0]; // Lấy sheet đầu tiên

                foreach (DataRow row in table.Rows)
                {
                    yield return new object[]
                    {
                        row["Number1"].ToString().Trim(),
                        row["Number2"].ToString().Trim(),
                        row["ExpectedResult"].ToString().Trim()
                    };
                }
            }
        }
    }
}