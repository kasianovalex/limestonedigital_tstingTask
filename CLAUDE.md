# CLAUDE.md

Guidance for Claude (or any AI coding agent) working in this repo.

## Project

Practical QA test task for Limestone Digital — a small C#/.NET framework with one UI test
(saucedemo.com: login → add to cart → verify cart) and one API test (jsonplaceholder.typicode.com:
GET + contract assertions). The goal is not coverage — it's a clean, defensible architecture with
a documented rationale.

## Stack

- .NET 8, NUnit
- Selenium WebDriver — UI (driver binary resolved by Selenium Manager, no WebDriverManager)
- RestSharp — API
- Reqnroll — BDD feature file for the UI scenario only
- Allure.NUnit — reporting

## Commands

- Build: `dotnet build`
- Run full suite: `dotnet test`
- Run one test: `dotnet test --filter "FullyQualifiedName~ClassName.MethodName"`
- Report: TRX under `TestResults/`; HTML via `dotnet test --logger "html;LogFileName=report.html"`;
  Allure results under `bin/Debug/net8.0/allure-results/`, rendered with
  `allure generate allure-results --clean -o allure-report` (Allure CLI, needs a JRE)

## Architecture (3 layers — target design, only partially built in this submission)

1. **Test layer** — NUnit `[Test]` methods / Reqnroll `.feature` files. Reads like a scenario.
   No Selenium/RestSharp calls, no selectors, no assertions on raw HTTP responses here.
2. **Business layer** — step methods grouped by concern: `ApiSteps`, `UiSteps`, `AssertionSteps`,
   and any other business-logic/calculation steps. Orchestrates the Core layer. No driver or
   HTTP client code lives here.
3. **Core layer** — split in two:
   - **SUT part** (knows *this* system under test): API clients, UI page objects, connectors,
     system entities/DTOs.
   - **TAS part** (system-agnostic test automation plumbing): driver factory/lifecycle, logger,
     custom matchers, generic helpers.

Dependency direction is one-way: Test → Business → Core. Nothing in Core references Business or
Test. Nothing in SUT references TAS internals beyond what TAS exposes (driver instance, http
client, logger).

## Hard rules

- Tests never call `IWebDriver` / `RestClient` directly — only through the Business layer.
- Page objects and API clients live only under `Core/SUT` — never instantiated from the Test layer.
- No hard-coded URLs, users, or secrets in test code — pulled from `appsettings.{env}.json`;
  secrets from environment variables / CI secret store, never committed.
- Tests are independent and parallel-safe: no shared mutable state, no execution-order assumptions.
- One logical assertion concern per test; reusable assertions go through a Core/TAS matcher, not
  copy-pasted `Assert.That` blocks.

## Naming

- Namespace root: `LimestoneDigital.QA.*` (e.g. `LimestoneDigital.QA.Core.Sut.Ui.Pages`)
- One page object per page, one API client per resource.
- Test names: `MethodUnderTest_Scenario_ExpectedResult`.

## Implemented vs. described-only in this submission

**Implemented:** minimal Test/Business/Core split, one UI scenario (Reqnroll feature +
step bindings), one API test class, driver factory, three page objects, one API client, layered
config, Core/TAS matchers, Allure reporting.

**Described only in `FrameworkStructure.md`, not built:** CI workflow, Docker/grid execution,
secrets management beyond the env-var override, parallel execution config, screenshot-on-failure
attachments, cross-run Allure history/trend, test data builders.

## When extending this repo

- Adding a new UI check → new/extended page object under `Core/SUT/Ui/Pages`, new step(s) under
  `Business/UiSteps`, new `[Test]` or feature step under `Tests`.
- Adding a new API check → new/extended client under `Core/SUT/Api`, new step(s) under
  `Business/ApiSteps`.
- Never add a new top-level layer without updating this file and `FrameworkStructure.md`
  (which holds the layer/ownership answer the submission is graded on).
