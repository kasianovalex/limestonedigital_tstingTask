using OpenQA.Selenium;

namespace LimestoneDigital.QA.Core.Sut.Ui.Pages;

public class InventoryPage : BasePage
{
    private static readonly By CartLink = By.ClassName("shopping_cart_link");

    public InventoryPage(IWebDriver driver) : base(driver)
    {
    }

    public void AddItemToCart(string itemName)
    {
        var slug = itemName.ToLowerInvariant().Replace(" ", "-");
        var addToCartButton = By.Id($"add-to-cart-{slug}");
        Wait.Until(d => d.FindElement(addToCartButton)).Click();
    }

    public void OpenCart() => Driver.FindElement(CartLink).Click();
}
