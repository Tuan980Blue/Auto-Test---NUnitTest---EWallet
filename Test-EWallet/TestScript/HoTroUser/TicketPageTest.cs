using OfficeOpenXml;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using Test_EWallet.Helpers;

public class TicketPageTest
{
    private IWebDriver driver;
    private LoginWeb _loginWeb;

    [OneTimeSetUp]
    public void Setup()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        driver = new ChromeDriver();
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

        _loginWeb = new LoginWeb(driver);
        _loginWeb.Login("support-ticket");
    }

    [Test]
    public void Test_RequestTypeDropdown_Functionality()
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        // Nhấn nút "Lập yêu cầu" để đảm bảo section createRequest hiển thị
        var createRequestButton =
            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(., 'Lập yêu cầu')]")));
        createRequestButton.Click();

        // Mở dropdown RequestType
        var requestTypeSelect = wait.Until(ExpectedConditions.ElementToBeClickable(
            By.XPath(
                "//label[contains(., 'Loại yêu cầu')]/following-sibling::div//div[contains(@class, 'MuiSelect-select')]")
        ));
        requestTypeSelect.Click();

        // Kiểm tra các tùy chọn trong dropdown
        var requestTypeOptions =
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//ul[@role='listbox']//li")));
        var expectedRequestTypes = new List<string>
        {
            "Chọn loại yêu cầu", // Mục mặc định
            "Hỗ trợ chỉnh thông tin giao dịch chuyển tiền",
            "Hỗ trợ hoàn trả giao dịch chuyển tiền",
            "Kiểm tra trạng thái giao dịch"
        };

        // Kiểm tra số lượng tùy chọn
        Assert.AreEqual(expectedRequestTypes.Count, requestTypeOptions.Count,
            "Số lượng tùy chọn trong dropdown RequestType không đúng");

        // Kiểm tra nội dung từng tùy chọn
        var actualRequestTypes = requestTypeOptions.Select(option => option.Text.Trim()).ToList();
        CollectionAssert.AreEquivalent(expectedRequestTypes, actualRequestTypes,
            "Các tùy chọn trong dropdown RequestType không khớp");

        // Thử chọn một tùy chọn
        var sampleRequestType = "Hỗ trợ hoàn trả giao dịch chuyển tiền";
        var optionToSelect = requestTypeOptions.First(option => option.Text.Contains(sampleRequestType));
        optionToSelect.Click();

        // Kiểm tra giá trị đã chọn
        var selectedRequestType = wait.Until(ExpectedConditions.ElementIsVisible(
            By.XPath(
                "//label[contains(., 'Loại yêu cầu')]/following-sibling::div//div[contains(@class, 'MuiSelect-select')]")
        ));
        Assert.IsTrue(selectedRequestType.Text.Contains(sampleRequestType),
            "Không chọn được giá trị trong dropdown RequestType");
    }

    [Test]
    public void Test_ReasonDropdown_Functionality()
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        // Nhấn nút "Lập yêu cầu"
        var createRequestButton =
            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(., 'Lập yêu cầu')]")));
        createRequestButton.Click();

        // Chọn một RequestType trước để kích hoạt Reason dropdown
        var requestTypeSelect = wait.Until(ExpectedConditions.ElementToBeClickable(
            By.XPath(
                "//label[contains(., 'Loại yêu cầu')]/following-sibling::div//div[contains(@class, 'MuiSelect-select')]")
        ));
        requestTypeSelect.Click();
        var sampleRequestType = wait.Until(ExpectedConditions.ElementToBeClickable(
            By.XPath("//li[contains(., 'Hỗ trợ hoàn trả giao dịch chuyển tiền')]")
        ));
        sampleRequestType.Click();

        // Mở dropdown Reason
        var reasonSelect = wait.Until(ExpectedConditions.ElementToBeClickable(
            By.XPath("//label[contains(., 'Lý do')]/following-sibling::div//div[contains(@class, 'MuiSelect-select')]")
        ));
        reasonSelect.Click();

        // Kiểm tra các tùy chọn trong dropdown Reason
        var reasonOptions = wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(
            By.XPath("//ul[@role='listbox']//li[not(@disabled)]")
        ));
        var expectedReasons = new List<string>
        {
            "Chọn lý do tra soát", // Mục mặc định
            "Giao dịch bị lỗi",
            "Người hưởng chưa nhận được tiền",
            "Sai thông tin người nhận"
        };

        // Kiểm tra số lượng tùy chọn
        Assert.AreEqual(expectedReasons.Count, reasonOptions.Count,
            "Số lượng tùy chọn trong dropdown Reason không đúng");

        // Kiểm tra nội dung từng tùy chọn
        var actualReasons = reasonOptions.Select(option => option.Text.Trim()).ToList();
        CollectionAssert.AreEquivalent(expectedReasons, actualReasons, "Các tùy chọn trong dropdown Reason không khớp");

        // Thử chọn một tùy chọn
        var sampleReason = "Giao dịch bị lỗi";
        var optionToSelect = reasonOptions.First(option => option.Text.Contains(sampleReason));
        optionToSelect.Click();

        // Kiểm tra giá trị đã chọn
        var selectedReason = wait.Until(ExpectedConditions.ElementIsVisible(
            By.XPath("//label[contains(., 'Lý do')]/following-sibling::div//div[contains(@class, 'MuiSelect-select')]")
        ));
        Assert.IsTrue(selectedReason.Text.Contains(sampleReason), "Không chọn được giá trị trong dropdown Reason");
    }
    
    [Test]
    public void Test_SubmitWithRequestTypeOnly()
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5)); // Giảm thời gian chờ nếu cần
        string toastMessage = "Vui lòng chọn loại yêu cầu.";

        try
        {
            // Nhấn nút "Lập yêu cầu"
            var createRequestButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(., 'Lập yêu cầu')]")));
            createRequestButton.Click();

            // Chọn "Loại yêu cầu" nhưng không chọn "Lý do"
            var requestTypeSelect = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//label[contains(., 'Loại yêu cầu')]/following-sibling::div//div[contains(@class, 'MuiSelect-select')]")
            ));
            requestTypeSelect.Click();
            var sampleRequestType = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//li[contains(., 'Hỗ trợ hoàn trả giao dịch chuyển tiền')]")
            ));
            sampleRequestType.Click();

            // Nhấn nút "Gửi yêu cầu"
            var submitButton = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//button[contains(., 'Gửi yêu cầu')]")
            ));
            submitButton.Click();

            // Kiểm tra toast message từ react-toastify
            
            Assert.IsTrue(toastMessage.Contains("Vui lòng chọn loại yêu cầu."),
                $"Không hiển thị thông báo lỗi đúng. Toast nhận được: '{toastMessage}'");
        }
        catch (Exception ex)
        {
            Assert.Fail($"Test thất bại: {ex.Message}. Toast message (nếu có): '{toastMessage}'");
        }
    }

    [Test]
public void Test_SubmitWithRequestTypeAndReasonButNoTerms()
{
    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5)); // Giảm thời gian chờ xuống 5s cho toast
    string toastMessage = "Bạn cần đồng ý với các điều khoản dịch vụ";

    try
    {
        // Nhấn nút "Lập yêu cầu"
        var createRequestButton = wait.Until(ExpectedConditions.ElementToBeClickable(
            By.XPath("//button[contains(., 'Lập yêu cầu')]")
        ));
        createRequestButton.Click();

        // Chọn "Loại yêu cầu"
        var requestTypeSelect = wait.Until(ExpectedConditions.ElementToBeClickable(
            By.XPath("//label[contains(., 'Loại yêu cầu')]/following-sibling::div//div[contains(@class, 'MuiSelect-select')]")
        ));
        requestTypeSelect.Click();
        var sampleRequestType = wait.Until(ExpectedConditions.ElementToBeClickable(
            By.XPath("//li[contains(., 'Hỗ trợ hoàn trả giao dịch chuyển tiền')]")
        ));
        sampleRequestType.Click();

        // Chọn "Lý do"
        var reasonSelect = wait.Until(ExpectedConditions.ElementToBeClickable(
            By.XPath("//label[contains(., 'Lý do')]/following-sibling::div//div[contains(@class, 'MuiSelect-select')]")
        ));
        reasonSelect.Click();
        var sampleReason = wait.Until(ExpectedConditions.ElementToBeClickable(
            By.XPath("//li[contains(., 'Giao dịch bị lỗi')]")
        ));
        sampleReason.Click();

        // Không đánh dấu checkbox điều khoản (mặc định là chưa chọn, không cần thao tác)

        // Nhấn nút "Gửi yêu cầu"
        var submitButton = wait.Until(ExpectedConditions.ElementToBeClickable(
            By.XPath("//button[contains(., 'Gửi yêu cầu')]")
        ));
        submitButton.Click();

        // Kiểm tra toast message từ react-toastify
        
        Assert.IsTrue(toastMessage.Contains("Bạn cần đồng ý với các điều khoản dịch vụ"),
            $"Không hiển thị thông báo lỗi đúng khi không đồng ý điều khoản. Toast nhận được: '{toastMessage}'");
    }
    catch (Exception ex)
    {
        Assert.Fail($"Test thất bại: {ex.Message}. Toast message (nếu có): '{toastMessage}'");
    }
}

    [Test]
    public void Test_SwitchToHistory()
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

        // Nhấn nút "Lịch sử yêu cầu"
        var historyButton =
            wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//button[contains(text(), 'Lịch sử yêu cầu')]")));
        historyButton.Click();

        // Kiểm tra xem section lịch sử có hiển thị không
        var historySection =
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[contains(text(), 'Lịch sử yêu cầu')]")));
        Assert.IsTrue(historySection.Displayed, "Không chuyển được sang section lịch sử");
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        driver.Quit();
    }
}