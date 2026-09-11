using Allure.Net.Commons;
using LimestoneDigital.QA.Business;
using Reqnroll;

namespace LimestoneDigital.QA.Tests.Ui;

[Binding]
public class AddItemToCartSteps
{
    private readonly UiSteps _uiSteps = new();
    private readonly AssertionSteps _assertions = new();
    private IReadOnlyList<string> _cartItems = Array.Empty<string>();

    [Given(@"I am logged in as the standard user")]
    public void GivenIAmLoggedInAsTheStandardUser()
    {
        AllureApi.Step("Log in as the standard user", () => _uiSteps.LoginAsStandardUser());
    }

    [When(@"I add ""(.*)"" to the cart")]
    public void WhenIAddItemToTheCart(string itemName)
    {
        AllureApi.Step($"Add '{itemName}' to the cart", () => _uiSteps.AddItemToCart(itemName));
    }

    [When(@"I open the cart")]
    public void WhenIOpenTheCart()
    {
        AllureApi.Step("Open the cart", () => _cartItems = _uiSteps.OpenCartAndGetItemNames());
    }

    [Then(@"the cart should contain ""(.*)""")]
    public void ThenTheCartShouldContain(string itemName)
    {
        AllureApi.Step($"Verify the cart contains '{itemName}'",
            () => _assertions.AssertCartContainsItem(_cartItems, itemName));
    }

    [AfterScenario]
    public void TearDown() => _uiSteps.Dispose();
}
