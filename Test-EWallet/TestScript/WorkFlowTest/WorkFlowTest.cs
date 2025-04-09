using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using Test_EWallet.Helpers;

namespace Test_EWallet.TestScript.WorkFlowTest
{
    public class WorkFlowTest
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        private readonly string baseUrl = "http://localhost:3000";
        private readonly string ssoUrl = "https://sso.htilssu.id.vn";
        private readonly string email = "testuser" + DateTime.Now.Ticks + "@gmail.com";
        private readonly string password = "Test@123";
        private readonly string lastName = "Nguyen";
        private readonly string firstName = "Van A";
        private readonly string dob = "08/04/2007";
        private readonly string cardNumber = "9704000000000018";
        private readonly string cardHolderName = "NGUYEN VAN A";
        private readonly string expiryDate = "12/25";
        private readonly string amount = "50000";
        private readonly string emailreceive = "tuanmeo980provip@gmail.com";

        [OneTimeSetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        [Test]
        public void Test_CompleteWorkflow()
        {
            // Step 1: Đăng ký tài khoản
            RegisterAccount();

            // Step 2: Đăng nhập
            Login();

            // Step 3: Liên kết thẻ
            LinkCard();

            // Step 4: Nạp tiền
            TopUpMoney();

            // Step 5: Chuyển tiền
            PerformTransfer( amount,  emailreceive,  "Chuyển tiền cho bạn bè");
        }

        private void RegisterAccount()
        {
            driver.Navigate().GoToUrl($"{ssoUrl}/sign-up");

            // Điền thông tin đăng ký
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//input[@placeholder='Nhập email']"))).SendKeys(email);
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//input[@placeholder='Nhập họ']"))).SendKeys(lastName);
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//input[@placeholder='Nhập tên']"))).SendKeys(firstName);
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//input[@placeholder='Nhập mật khẩu']"))).SendKeys(password);
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//input[@placeholder='Nhập lại mật khẩu']"))).SendKeys(password);
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//input[@placeholder='Chọn ngày sinh']"))).SendKeys(dob);

            // Check vào checkbox đồng ý điều khoản
            var termCheckbox = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("term")));
            termCheckbox.Click();

            // Click nút đăng ký - sử dụng selector chính xác hơn
            var registerButton = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//button[@type='submit']//span[contains(@class, 'mantine-Button-label') and contains(text(), 'Đăng ký')]")));
            registerButton.Click();
            
            //sau khi đăng ký thành công sẽ chuyển hướng về trang có url là
            // https://sso.htilssu.id.vn/
           
            
            // Đợi thêm 2 giây để đảm bảo trang đăng nhập đã tải hoàn toàn
            Thread.Sleep(2000);
        }

        private void Login()
        {
            driver.Navigate().GoToUrl($"{baseUrl}/bank");
            Thread.Sleep(2000);
            // Điều hướng đến trang đăng nhập
            driver.Navigate().GoToUrl($"https://sso.htilssu.id.vn/sign-in?returnUrl=http://localhost:3000/sso/callback&serviceId=WOW&callbackUrl=undefined");

            Thread.Sleep(2000);
            
            // Chỉ cần nhấn nút Tiếp tục trên màn hình, tại vì khi đăng ký xong sẽ tự động trả token về để đăng nhập
            var continueButton = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//button[.//span[contains(@class, 'mantine-Button-label') and contains(text(), 'Tiếp tục')]]")));
            continueButton.Click();
        }

        private void LinkCard()
        {
            // Click vào nút "Thêm thẻ ATM"
            var addCardButton = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//div[contains(@class, 'card-atm')]//h5[contains(text(), 'Thêm thẻ ATM')]")));
            addCardButton.Click();

            // Đợi form thêm thẻ hiển thị và điền thông tin thẻ
            // Chọn ngân hàng VietinBank
            var bankSelect = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//input[@data-path='bankId']")));
            bankSelect.Click();
            
            // Chọn VietinBank từ danh sách
            var vietinBankOption = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//div[contains(@class, 'mantine-Select-option') and @value='VietinBank']")));
            vietinBankOption.Click();
            
            // Điền tên chủ thẻ
            var holderNameInput = wait.Until(ExpectedConditions.ElementIsVisible(
                By.XPath("//input[@data-path='holderName']")));
            holderNameInput.SendKeys(cardHolderName);
            
            // Điền số thẻ
            var cardNumberInput = wait.Until(ExpectedConditions.ElementIsVisible(
                By.XPath("//input[@data-path='cardNumber']")));
            cardNumberInput.SendKeys(cardNumber);
            
            // Điền ngày hết hạn
            var expiryInputs = driver.FindElements(By.XPath("//div[contains(@class, 'flex justify-between items-center border-1')]//input"));
            if (expiryInputs.Count >= 2)
            {
                // Tách tháng và năm từ expiryDate (format: MM/YY)
                string[] expiryParts = expiryDate.Split('/');
                if (expiryParts.Length >= 2)
                {
                    expiryInputs[0].SendKeys(expiryParts[0]); // Tháng
                    expiryInputs[1].SendKeys(expiryParts[1]); // Năm
                }
            }

            // Click nút cập nhật
            var updateButton = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//button[.//span[contains(@class, 'mantine-Button-label') and contains(text(), 'Cập nhật')]]")));
            updateButton.Click();
            
            // Kiểm tra thông tin thẻ sau khi liên kết thành công
            VerifyCardInformation();
        }
        
        private void VerifyCardInformation()
        {
            // Đợi cho thẻ hiển thị
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//div[contains(@class, 'relative transform-gpu rounded-lg')]")));
            
            // Kiểm tra tên ngân hàng
            var bankNameElement = driver.FindElement(By.XPath("//div[contains(@class, 'w-full') and contains(text(), 'VietinBank')]"));
            Assert.That(bankNameElement.Text, Is.EqualTo("VietinBank"), "Tên ngân hàng không đúng");
            
            // Kiểm tra số thẻ
            var cardNumberElement = driver.FindElement(By.XPath("//div[contains(@class, 'text-xl text-center')]"));
            Assert.That(cardNumberElement.Text.Replace(" ", ""), Is.EqualTo(cardNumber), "Số thẻ không đúng");
            
            // Kiểm tra tên chủ thẻ
            var cardHolderElement = driver.FindElement(By.XPath("//div[contains(@class, 'line-clamp-1')]"));
            Assert.That(cardHolderElement.Text, Is.EqualTo(cardHolderName), "Tên chủ thẻ không đúng");
        }

        private void TopUpMoney()
        {
            driver.Navigate().GoToUrl($"{baseUrl}/top-up");

            // Nhập số tiền nạp
            wait.Until(ExpectedConditions.ElementIsVisible(
                By.XPath("//input[@placeholder='Số tiền tối thiểu phải lớn hơn 10.000 VND']"))).SendKeys("100000");

            // Chọn phương thức thanh toán
            var paymentMethod = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//p[contains(text(),'Online bằng thẻ liên kết')]")));
            paymentMethod.Click();

            // Chọn thẻ đã liên kết
            var cardList = driver.FindElements(By.XPath("//div[contains(@class, 'w-full flex items-center rounded')]"));
            if (cardList.Count > 0)
            {
                cardList[0].Click();
            }

            // Click nút nạp tiền
            var topUpButton = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//button[.//span[contains(text(),'Nạp tiền')]]")));
            topUpButton.Click();

            // Đợi và xác nhận nạp tiền thành công
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//div[contains(text(),'Nạp tiền thành công')]")));
        }

        private void PerformTransfer(string amount, string emailreceive, string content)
        {
            Thread.Sleep(2000);
            driver.Navigate().GoToUrl($"{baseUrl}/transfer");

            // Nhập số tiền
            var amountInput = wait.Until(ExpectedConditions.ElementIsVisible(
                By.XPath("//input[@placeholder='Số tiền tối thiểu phải lớn hơn 10.000 VND']")));
            amountInput.Clear();
            amountInput.SendKeys(amount);

            // Nhập email người nhận
            var emailInput = wait.Until(ExpectedConditions.ElementIsVisible(
                By.XPath("//input[@placeholder='Nhập email người nhận']")));
            emailInput.Clear();
            emailInput.SendKeys(emailreceive);

            // Nhập nội dung chuyển tiền
            var contentInput = wait.Until(ExpectedConditions.ElementIsVisible(
                By.XPath("//textarea[@placeholder='Nhập nội dung chuyển tiền']")));
            contentInput.Clear();
            contentInput.SendKeys(content);

            // Click nút xác nhận trên form chính
            var transferButton = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//button[contains(@class, 'mantine-Button-root') and contains(., 'Xác Nhận')]")));
            transferButton.Click();
            
            //kiểm tra xem có thông báo lỗi nào không
            string actualResult = "Chuyển tiền thành công";

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
            else
            {
                // Xác nhận OTP // Giả sử mã OTP là 123456
                var otpInputs = driver.FindElements(By.XPath("//input[@class='m_8fb7ebe7 mantine-Input-input mantine-PinInput-input']"));
                if (otpInputs.Count >= 6)
                {
                    // Nhập từng chữ số của mã OTP (123456)
                    otpInputs[0].SendKeys("1");
                    otpInputs[1].SendKeys("2");
                    otpInputs[2].SendKeys("3");
                    otpInputs[3].SendKeys("4");
                    otpInputs[4].SendKeys("5");
                    otpInputs[5].SendKeys("6");
                }

                // Click nút xác nhận
                var confirmButton = wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//button[contains(@class, 'mantine-Button-root') and contains(., 'Xác nhận')]")));
                confirmButton.Click();
                
                // Đợi chuyển đến trang chi tiết giao dịch, kiểm tra thông tin giao dịch
                Thread.Sleep(2000);
                VerifyTransactionDetails();
            }

           
            Assert.AreEqual("Chuyển tiền thành công", actualResult, $"Kết quả của {amount} sai");
        }
        
        private void VerifyTransactionDetails()
        {
            // Đợi cho trang chi tiết giao dịch hiển thị
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//div[contains(@class, 'mantine-Card-root')]//h2[contains(text(), 'Chi tiết giao dịch')]")));
            
            // Kiểm tra trạng thái giao dịch
            var statusElement = driver.FindElement(By.XPath("//span[contains(@class, 'text-green-500')]"));
            Assert.That(statusElement.Text, Is.EqualTo("Thành công"), "Trạng thái giao dịch không đúng");
            
            // Kiểm tra mã giao dịch
            var transactionIdElement = driver.FindElement(By.XPath("//p[contains(@class, 'font-medium') and contains(text(), '2025')]"));
            Assert.That(transactionIdElement.Text, Is.Not.Empty, "Mã giao dịch không được hiển thị");
            
            // Kiểm tra số tiền
            var amountElement = driver.FindElement(By.XPath("//p[contains(@class, 'text-2xl font-bold text-green-600')]"));
            Assert.That(amountElement.Text, Is.EqualTo("50.000 VND"), "Số tiền giao dịch không đúng");
            
            // Kiểm tra người nhận
            var recipientElement = driver.FindElement(By.XPath("//div[contains(@class, 'flex items-center space-x-2')]//p[contains(@class, 'font-medium')]"));
            Assert.That(recipientElement.Text, Is.EqualTo("Tuan Anh"), "Tên người nhận không đúng");
            
            // Kiểm tra nội dung chuyển khoản
            var contentElement = driver.FindElement(By.XPath("//p[contains(@class, 'font-medium') and contains(text(), 'Chuyển tiền')]"));
            Assert.That(contentElement.Text, Is.EqualTo("Chuyển tiền cho bạn bè"), "Nội dung chuyển khoản không đúng");
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            driver.Quit();
        }
    }
} 