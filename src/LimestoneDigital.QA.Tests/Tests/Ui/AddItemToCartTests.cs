using LimestoneDigital.QA.Business;
using NUnit.Framework;

namespace LimestoneDigital.QA.Tests.Ui;

[TestFixture]
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
        _uiSteps.LoginAsStandardUser();
        _uiSteps.AddItemToCart(ItemUnderTest);

        var cartItems = _uiSteps.OpenCartAndGetItemNames();

        _assertions.AssertCartContainsItem(cartItems, ItemUnderTest);
    }
}
