namespace Test_EWallet.Test_NapTien
{
    public class FileUtil
    {
        public static bool IsFileInUse(string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    // Nếu có thể mở file mà không có bất kỳ quyền chia sẻ nào, thì file không bị khóa
                    return false;
                }
            }
            catch (IOException)
            {
                // Nếu file bị khóa bởi một tiến trình khác, IOException sẽ được ném ra
                return true;
            }
        }
    }
}