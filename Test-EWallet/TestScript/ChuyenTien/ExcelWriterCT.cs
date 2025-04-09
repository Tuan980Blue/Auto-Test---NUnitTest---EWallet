using OfficeOpenXml;

namespace Test_EWallet.Test_NapTien
{
    public class ExcelWriterCT
    {
        public static void WriteTestResult(string filePath, string amount, string expected, string actual, string status)
        {
            if (FileUtil.IsFileInUse(filePath))
            {
                throw new Exception("⚠️ File Excel đang được sử dụng bởi một tiến trình khác. Vui lòng đóng file Excel và thử lại.");
            }
            
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            FileInfo fileInfo = new FileInfo(filePath);
    
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException($"⚠️ File Excel không tồn tại: {filePath}");
            }

            using (var package = new ExcelPackage(fileInfo))
            {
                // Kiểm tra xem workbook có sheet nào không
                if (package.Workbook.Worksheets.Count == 0)
                {
                    throw new Exception("⚠️ File Excel không chứa sheet nào.");
                }

                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                int rowCount = worksheet.Dimension?.Rows ?? 0; // Kiểm tra nếu file rỗng
                if (rowCount == 0)
                {
                    throw new Exception("⚠️ File Excel không có dữ liệu.");
                }

                bool found = false;
                for (int row = 2; row <= rowCount + 1; row++) // Bắt đầu từ hàng 2, đến rowCount + 1 để bao gồm 7 hàng
                {
                    // Tìm hàng dựa trên cột Amount (cột 1)
                    if (worksheet.Cells[row, 1].Text == amount)
                    {
                        // Cập nhật Actual (cột 6) nếu giá trị mới khác giá trị hiện tại
                        if (worksheet.Cells[row, 6].Text != actual)
                        {
                            worksheet.Cells[row, 6].Value = actual;
                        }
                        
                        // Cập nhật Status (cột 7) nếu giá trị mới khác giá trị hiện tại
                        if (worksheet.Cells[row, 7].Text != status)
                        {
                            worksheet.Cells[row, 7].Value = status;
                        }

                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    throw new Exception("⚠️ Không tìm thấy dòng dữ liệu tương ứng để cập nhật.");
                }

                package.Save();
            }
        }
    }
}