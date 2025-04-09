# 🚀 Auto Test - NUnitTest - EWallet

![GitHub repo size](https://img.shields.io/github/repo-size/Tuan980Blue/Auto-Test---NUnitTest---EWallet)
![GitHub stars](https://img.shields.io/github/stars/Tuan980Blue/Auto-Test---NUnitTest---EWallet?style=social)
![Last Commit](https://img.shields.io/github/last-commit/Tuan980Blue/Auto-Test---NUnitTest---EWallet)
![License](https://img.shields.io/github/license/Tuan980Blue/Auto-Test---NUnitTest---EWallet)

> Bộ mã tự động kiểm thử (Automation Testing) cho ứng dụng ví điện tử (**EWallet**) sử dụng **Selenium + NUnit**.  
> Mục tiêu: đảm bảo chất lượng các chức năng chính của hệ thống qua các bài test tự động hoá.

---

## 🛠️ Công nghệ sử dụng

- 🧪 **NUnit** - Framework kiểm thử cho C#
- 🕸️ **Selenium WebDriver** - Tự động hoá trình duyệt
- ⚙️ **.NET Framework / .NET Core** (tuỳ phiên bản bạn dùng)
- 🧱 **Page Object Model (POM)** - Thiết kế test dễ bảo trì

---

## 📂 Cấu trúc thư mục

```plaintext
Test-EWallet/
├── Pages/               # Các trang tương tác (POM)
├── Tests/               # Các file test NUnit
├── Drivers/             # Setup & teardown WebDriver
├── Utils/               # Hàm hỗ trợ (helper functions)
├── Reports/             # (Tùy chọn) Báo cáo test
├── appsettings.json     # Config (URL, credentials,...)
└── README.md            # Tệp giới thiệu project
