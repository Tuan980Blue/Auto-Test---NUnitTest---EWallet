using OfficeOpenXml;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using Test_EWallet.Helpers;

namespace Test_EWallet.Test_NapTien
{
    public class NapTienTest
    {
        private IWebDriver driver;
        private LoginWeb _loginWeb;

        private static readonly string excelFilePath =
            "D:\\Projects\\BDCL_PM\\Selenium-EWallet\\Test-EWallet\\Test-EWallet\\FileExcels\\NapTien.xlsx";

        [OneTimeSetUp]
        public void Setup()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            driver = new ChromeDriver();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            _loginWeb = new LoginWeb(driver);

            _loginWeb.Login("top-up");
        }

        public static IEnumerable<object[]> GetTestDataFromExcel()
        {
            return ExcelDataProvider.ReadExcel(excelFilePath);
        }

        [Test, TestCaseSource(nameof(GetTestDataFromExcel))]
        public void Test_NapTien(string amount, string selectPaymentMethod, string expectedResult)
        {
            string actualResult = "Nạp tiền thành công";

            PerformTopUp(amount, selectPaymentMethod.ToLower() == "true");

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

        private void PerformTopUp(string amount, bool selectPaymentMethod = true)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            var amountInput =
                wait.Until(ExpectedConditions.ElementIsVisible(
                    By.XPath("//input[@placeholder='Số tiền tối thiểu phải lớn hơn 10.000 VND']")));
            amountInput.Clear();
            amountInput.SendKeys(amount);

            if (selectPaymentMethod)
            {
                var paymentMethod =
                    wait.Until(ExpectedConditions.ElementToBeClickable(
                        By.XPath("//p[contains(text(),'Online bằng thẻ liên kết')]")));
                paymentMethod.Click();

                var cardList = driver.FindElements(By.XPath("//div[contains(@class, 'w-full flex items-center rounded')]"));
                if (cardList.Count > 0)
                {
                    cardList[0].Click();
                }
            }

            var topUpButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[.//span[contains(text(),'Nạp tiền')]]")));
            topUpButton.Click();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            driver.Quit();
        }
    }
}