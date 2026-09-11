using Allure.NUnit;
using Allure.NUnit.Attributes;

namespace LimestoneDigital.QA.Tests.Ui;

/// <summary>
/// Reqnroll generates AddItemToCart.feature.cs -> partial class ShoppingCartFeature (named
/// after the Gherkin Feature title). C# merges attributes across partial declarations, so this
/// hand-written half attaches Allure reporting without touching the generated file.
/// </summary>
[AllureNUnit]
[AllureSuite("UI")]
public partial class ShoppingCartFeature
{
}
