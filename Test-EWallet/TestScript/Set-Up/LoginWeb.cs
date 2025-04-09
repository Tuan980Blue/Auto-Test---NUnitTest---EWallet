using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace Test_EWallet.Helpers
{
    // Class mới để xử lý login
    public class LoginWeb
    {
        private readonly IWebDriver driver;
        private readonly string email = "tuanmeo980provip@gmail.com";
        private readonly string password;
        private readonly WebDriverWait wait;

        public LoginWeb(IWebDriver driver)
        {
            this.driver = driver;
            this.password = Environment.GetEnvironmentVariable("E_WALLET_PASSWORD") ?? "Password123!";
            this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        public void Login(string url)
        {
            // Điều hướng đến trang đăng nhập
            driver.Navigate().GoToUrl($"http://localhost:3000/{url}");
            driver.Navigate()
                .GoToUrl(
                    "https://sso.htilssu.id.vn/sign-in?returnUrl=http://localhost:3000/sso/callback&serviceId=WOW&callbackUrl=undefined");

            // Điền thông tin đăng nhập
            var emailInput = driver.FindElement(By.XPath("//input[@placeholder='Nhập email']"));
            emailInput.Clear();
            emailInput.SendKeys(email);

            var pwInput = driver.FindElement(By.XPath("//input[@placeholder='Nhập mật khẩu']"));
            pwInput.Clear();
            pwInput.SendKeys(password);

            // Nhấn nút đăng nhập
            var loginButton =
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button span.m_811560b9")));
            loginButton.Click();
        }
    }
}