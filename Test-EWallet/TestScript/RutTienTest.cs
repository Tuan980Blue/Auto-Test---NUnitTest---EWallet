using OfficeOpenXml;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using Test_EWallet.Helpers;

namespace Test_EWallet.Test_NapTien
{
    public class RutTienTest
    {
        private IWebDriver driver;
        private LoginWeb _loginWeb;

        private static readonly string excelFilePath =
            "D:\\Projects\\BDCL_PM\\Selenium-EWallet\\Test-EWallet\\Test-EWallet\\FileExcels\\RutTien.xlsx";

        [OneTimeSetUp]
        public void Setup()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            driver = new ChromeDriver();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            _loginWeb = new LoginWeb(driver);

            _loginWeb.Login("withdraw");
        }

        public static IEnumerable<object[]> GetTestDataFromExcel()
        {
            return ExcelDataProvider.ReadExcel(excelFilePath);
        }

        [Test, TestCaseSource(nameof(GetTestDataFromExcel))]
        public void Test_RutTien(string amount, string selectPaymentMethod, string expectedResult)
        {
            string actualResult = "Rút tiền thành công";

            PerformWithDraw(amount, selectPaymentMethod.ToLower() == "true");

            var errorMessages = driver.FindElements(By.XPath("//*[contains(@class, 'mantine-NumberInput-error')]"));
            var paymentMethodError = driver.FindElements(By.XPath("//*[contains(@class, 'text-red-500')]"));

            if (errorMessages.Count > 0)
            {
                actualResult = errorMessages[0].Text;
            }
            else if (paymentMethodError.Count > 0)
            {
                actualResult = paymentMethodError[0].Text;
            }

            string status = expectedResult == actualResult ? "Pass" : "Fail";
            ExcelDataProvider.WriteTestResult(excelFilePath, amount, selectPaymentMethod, expectedResult, actualResult, status);
            Assert.AreEqual(expectedResult, actualResult, $"Kết quả của {amount} sai");
        }

        private void PerformWithDraw(string amount, bool selectPaymentMethod = true)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            var amountInput =
                wait.Until(ExpectedConditions.ElementIsVisible(
                    By.XPath("//input[@placeholder='Số tiền cần rút']")));
            amountInput.Clear();
            amountInput.SendKeys(amount);

            if (selectPaymentMethod)
            {
                //chọn thẻ rút về
                var cardList = driver.FindElements(By.XPath("//div[contains(@class, 'w-full flex items-center rounded')]"));
                if (cardList.Count > 0)
                {
                    cardList[0].Click();
                }
            }

            var withDrawButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[.//span[contains(text(),'Tiếp tục')]]")));
            withDrawButton.Click();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            driver.Quit();
        }
    }
}