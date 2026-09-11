using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace LimestoneDigital.QA.Core.Sut.Ui.Pages;

public abstract class BasePage
{
    protected readonly IWebDriver Driver;
    protected readonly WebDriverWait Wait;

    protected BasePage(IWebDriver driver)
    {
        Driver = driver;
        Wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
    }
}
