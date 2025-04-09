using OfficeOpenXml;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using Test_EWallet.Helpers;

namespace Test_EWallet.Test_NapTien
{
    public class ChuyenTienTest
    {
        private IWebDriver driver;
        private LoginWeb _loginWeb;

        private static readonly string excelFilePath = "D:\\Projects\\BDCL_PM\\Selenium-EWallet\\Test-EWallet\\Test-EWallet\\FileExcels\\ChuyenTien.xlsx";

        [OneTimeSetUp]
        public void Setup()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            driver = new ChromeDriver();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            _loginWeb = new LoginWeb(driver);

            _loginWeb.Login("transfer");
        }

        [SetUp]
        public void ResetPage()
        {
            driver.Navigate().Refresh(); // Làm mới trang để xóa dữ liệu cũ
        }

        public static IEnumerable<object[]> GetTestDataFromExcel()
        {
            return ExcelDataProviderCT.ReadExcel(excelFilePath);
        }

        [Test, TestCaseSource(nameof(GetTestDataFromExcel))]
        public void Test_ChuyenTien(string amount, string email, string content, string otp, string expectedResult)
        {
            string actualResult = "Chuyển tiền thành công";

            PerformTransfer(amount, email, content, otp);

            var moneyError = driver.FindElements(By.XPath("//*[contains(@class, 'mantine-NumberInput-error')]"));
            var emailError = driver.FindElements(By.XPath("//*[contains(@class, 'mantine-TextInput-error')]"));

            if (moneyError.Count > 0)
            {
                actualResult = moneyError[0].Text;
            }
            else if (emailError.Count > 0)
            {
                actualResult = emailError[0].Text;
            }

            string status = expectedResult == actualResult ? "Pass" : "Fail";
            ExcelDataProviderCT.WriteTestResult(excelFilePath, amount, expectedResult, actualResult, status);
            Assert.AreEqual(expectedResult, actualResult, $"Kết quả của {amount} sai");
        }

        private void PerformTransfer(string amount, string email, string content, string otp)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            // Nhập số tiền
            var amountInput = wait.Until(ExpectedConditions.ElementIsVisible(
                By.XPath("//input[@placeholder='Số tiền tối thiểu phải lớn hơn 10.000 VND']")));
            amountInput.Clear();
            amountInput.SendKeys(amount);

            // Nhập email người nhận
            var emailInput = wait.Until(ExpectedConditions.ElementIsVisible(
                By.XPath("//input[@placeholder='Nhập email người nhận']")));
            emailInput.Clear();
            emailInput.SendKeys(email);

            // Nhập nội dung chuyển tiền
            var contentInput = wait.Until(ExpectedConditions.ElementIsVisible(
                By.XPath("//textarea[@placeholder='Nhập nội dung chuyển tiền']")));
            contentInput.Clear();
            contentInput.SendKeys(content);

            // Click nút xác nhận trên form chính
            var transferButton = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//button[contains(@class, 'mantine-Button-root') and contains(., 'Xác Nhận')]")));
            transferButton.Click();

            //Xác nhận OTP để sau
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            driver.Quit();
        }
    }
}