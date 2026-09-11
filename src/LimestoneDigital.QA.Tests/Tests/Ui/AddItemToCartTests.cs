using Allure.Net.Commons;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using LimestoneDigital.QA.Business;
using NUnit.Framework;

namespace LimestoneDigital.QA.Tests.Ui;

[TestFixture]
[AllureNUnit]
[AllureSuite("UI")]
[Category("Smoke")]
[Category("Ui")]
public class AddItemToCartTests
{
    private UiSteps _uiSteps = null!;
    private AssertionSteps _assertions = null!;

    private const string ItemUnderTest = "Sauce Labs Backpack";

    [SetUp]
    public void SetUp()
    {
        _uiSteps = new UiSteps();
        _assertions = new AssertionSteps();
    }

    [TearDown]
    public void TearDown() => _uiSteps.Dispose();

    [Test]
    public void AddItemToCart_ValidLogin_ItemAppearsInCart()
    {
        AllureApi.Step("Log in as the standard user", () => _uiSteps.LoginAsStandardUser());
        AllureApi.Step($"Add '{ItemUnderTest}' to the cart", () => _uiSteps.AddItemToCart(ItemUnderTest));

        IReadOnlyList<string> cartItems = Array.Empty<string>();
        AllureApi.Step("Open the cart", () => cartItems = _uiSteps.OpenCartAndGetItemNames());

        AllureApi.Step("Verify the cart contains the added item",
            () => _assertions.AssertCartContainsItem(cartItems, ItemUnderTest));
    }
}
