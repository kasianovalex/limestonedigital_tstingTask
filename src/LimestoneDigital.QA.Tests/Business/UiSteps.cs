using LimestoneDigital.QA.Core.Sut.Ui.Pages;
using LimestoneDigital.QA.Core.Tas.Config;
using LimestoneDigital.QA.Core.Tas.Driver;
using OpenQA.Selenium;

namespace LimestoneDigital.QA.Business;

/// <summary>
/// Owns one WebDriver session for the lifetime of a test and orchestrates the page objects
/// behind it. Tests call this, never IWebDriver or a page object, directly.
/// </summary>
public class UiSteps : IDisposable
{
    private readonly IWebDriver _driver;
    private readonly UiSettings _settings;
    private readonly LoginPage _loginPage;
    private readonly InventoryPage _inventoryPage;
    private readonly CartPage _cartPage;

    public UiSteps()
    {
        _settings = ConfigurationProvider.Settings.Ui;
        _driver = DriverFactory.Create(_settings);
        _loginPage = new LoginPage(_driver, _settings.BaseUrl);
        _inventoryPage = new InventoryPage(_driver);
        _cartPage = new CartPage(_driver);
    }

    public void LoginAsStandardUser()
    {
        _loginPage.Open();
        _loginPage.Login(_settings.StandardUser, _settings.StandardPassword);
    }

    public void AddItemToCart(string itemName) => _inventoryPage.AddItemToCart(itemName);

    public IReadOnlyList<string> OpenCartAndGetItemNames()
    {
        _inventoryPage.OpenCart();
        return _cartPage.GetItemNames();
    }

    public void Dispose() => _driver.Quit();
}
