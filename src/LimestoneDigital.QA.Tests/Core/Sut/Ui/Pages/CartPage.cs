using OpenQA.Selenium;

namespace LimestoneDigital.QA.Core.Sut.Ui.Pages;

public class CartPage : BasePage
{
    private static readonly By ItemNames = By.ClassName("inventory_item_name");

    public CartPage(IWebDriver driver) : base(driver)
    {
    }

    public IReadOnlyList<string> GetItemNames()
    {
        Wait.Until(d => d.FindElements(ItemNames).Count > 0);
        return Driver.FindElements(ItemNames).Select(e => e.Text).ToList();
    }
}
