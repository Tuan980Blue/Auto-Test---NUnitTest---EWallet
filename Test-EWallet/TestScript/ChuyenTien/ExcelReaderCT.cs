using System.Data;
using ExcelDataReader;

namespace Test_EWallet.Test_NapTien
{
    public class ExcelReaderCT
    {
        public static IEnumerable<object[]> ReadExcel(string filePath)
        {
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

                    if (dataSet.Tables.Count == 0)
                    {
                        throw new Exception("⚠️ File Excel không chứa bảng dữ liệu nào.");
                    }

                    DataTable table = dataSet.Tables[0]; // Lấy sheet đầu tiên

                    foreach (DataRow row in table.Rows)
                    {
                        yield return new object[]
                        {
                            row["Amount"].ToString().Trim(),      // Số tiền
                            row["Email"].ToString().Trim(),       // Email người nhận
                            row["Content"].ToString().Trim(),     // Nội dung
                            row["OTP"].ToString().Trim(),         // Mã OTP
                            row["ExpectedResult"].ToString().Trim() // Kết quả mong đợi
                        };
                    }
                }
            }
        }
    }
}