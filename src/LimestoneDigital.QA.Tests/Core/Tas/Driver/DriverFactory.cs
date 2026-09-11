using LimestoneDigital.QA.Core.Tas.Config;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace LimestoneDigital.QA.Core.Tas.Driver;

/// <summary>
/// Single place that knows how to build and configure a WebDriver instance. Everything
/// downstream (pages, steps) depends on IWebDriver only, never on this factory or on Chrome
/// specifics, so swapping browser/grid/remote execution stays a one-file change.
/// </summary>
public static class DriverFactory
{
    public static IWebDriver Create(UiSettings settings)
    {
        new DriverManager().SetUpDriver(new ChromeConfig());

        var options = new ChromeOptions();
        if (settings.Headless)
        {
            options.AddArgument("--headless=new");
        }

        options.AddArgument("--window-size=1920,1080");
        options.AddArgument("--no-sandbox");

        var driver = new ChromeDriver(options);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(settings.ImplicitWaitSeconds);
        return driver;
    }
}
