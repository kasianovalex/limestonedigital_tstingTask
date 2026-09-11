using OpenQA.Selenium;

namespace LimestoneDigital.QA.Core.Sut.Ui.Pages;

public class LoginPage : BasePage
{
    private static readonly By UsernameInput = By.Id("user-name");
    private static readonly By PasswordInput = By.Id("password");
    private static readonly By LoginButton = By.Id("login-button");

    private readonly string _baseUrl;

    public LoginPage(IWebDriver driver, string baseUrl) : base(driver)
    {
        _baseUrl = baseUrl;
    }

    public void Open() => Driver.Navigate().GoToUrl(_baseUrl);

    public void Login(string username, string password)
    {
        Wait.Until(d => d.FindElement(UsernameInput)).SendKeys(username);
        Driver.FindElement(PasswordInput).SendKeys(password);
        Driver.FindElement(LoginButton).Click();
    }
}
