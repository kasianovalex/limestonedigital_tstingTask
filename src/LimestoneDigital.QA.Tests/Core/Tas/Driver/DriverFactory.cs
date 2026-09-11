using LimestoneDigital.QA.Core.Tas.Config;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace LimestoneDigital.QA.Core.Tas.Driver;

/// <summary>
/// Single place that knows how to build and configure a WebDriver instance. Everything
/// downstream (pages, steps) depends on IWebDriver only, never on this factory or on Chrome
/// specifics, so swapping browser/grid/remote execution stays a one-file change.
/// Driver binary resolution is left to Selenium Manager (built into Selenium.WebDriver since
/// 4.6), which matches the chromedriver version to the locally installed Chrome rather than
/// always grabbing latest-stable.
/// </summary>
public static class DriverFactory
{
    public static IWebDriver Create(UiSettings settings)
    {
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
