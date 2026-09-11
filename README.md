# LimestoneDigital QA — Automation Framework

Practical QA test task submission (Part 1). One UI scenario against
[saucedemo.com](https://www.saucedemo.com/) (login → add to cart → verify cart), written as a
Reqnroll/Gherkin feature, and one API test against
[jsonplaceholder.typicode.com](https://jsonplaceholder.typicode.com/) (GET a resource → assert the
response contract), built around a small Test → Business → Core layering intended to scale to a
real suite. See [`CLAUDE.md`](CLAUDE.md) for the layer contract this was built against.

## Stack

.NET 8, NUnit, Selenium WebDriver (Chrome, driver binary resolved automatically by Selenium
Manager), RestSharp, Reqnroll (Gherkin feature file for the UI scenario),
`Microsoft.Extensions.Configuration` for layered settings.

## Install & run

Prerequisites: .NET 8 SDK, Chrome installed locally (Selenium Manager, built into
Selenium.WebDriver, resolves the matching chromedriver at test run time — no manual driver setup
needed).

```bash
# restore + build
dotnet build

# run the full suite
dotnet test

# run a single test
dotnet test --filter "FullyQualifiedName~ShoppingCartFeature.AddAnItemToTheCart"
dotnet test --filter "FullyQualifiedName~GetPostTests.GetPost_ExistingId_ReturnsValidContract"

# run by category (Ui / Api / Smoke are tagged on both fixtures)
dotnet test --filter "Category=Smoke"
```

The UI scenario itself lives as Gherkin in
[`Tests/Ui/AddItemToCart.feature`](src/LimestoneDigital.QA.Tests/Tests/Ui/AddItemToCart.feature),
bound to step definitions in
[`AddItemToCartSteps.cs`](src/LimestoneDigital.QA.Tests/Tests/Ui/AddItemToCartSteps.cs); Reqnroll
generates the NUnit fixture (`ShoppingCartFeature`) from the feature file at build time.

By default the UI test runs Chrome headless. To watch it run headed locally, either edit
`appsettings.dev.json` (`Ui.Headless: false`, already set that way) and run with
`TEST_ENV=dev dotnet test`, or override a single value without touching any file:
`QA_Ui__Headless=false dotnet test` (see
[FrameworkStructure.md → Environment configuration & secrets](FrameworkStructure.md#environment-configuration--secrets)
for how the override chain works).

## Test report

NUnit/VSTest writes a `.trx` file under `TestResults/` on every `dotnet test` run. For a browsable
HTML report:

```bash
dotnet test --logger "html;LogFileName=report.html"
```

The report lands at `src/LimestoneDigital.QA.Tests/TestResults/report.html`.

### Allure report

`Allure.NUnit` is wired in on both fixtures (`[AllureNUnit]`), and the UI test's steps
(login → add to cart → open cart → verify) are broken out individually via `AllureApi.Step(...)`
so a failure shows which step it happened on, not just which test. Every `dotnet test` run writes
raw results to `allure-results/` under the test binary's output folder
(`src/LimestoneDigital.QA.Tests/bin/Debug/net8.0/allure-results/`).

Rendering that into the actual HTML report needs the Allure CLI (Java-based) on top:

```bash
npm install -g allure-commandline   # one-time; needs a JRE on PATH

cd src/LimestoneDigital.QA.Tests/bin/Debug/net8.0
allure generate allure-results --clean -o allure-report   # writes static HTML
allure open allure-report                                  # or: allure serve allure-results
```

Verified end to end this session (installed a JRE + `allure-commandline` and generated a real
report from a live `dotnet test` run) — this isn't just wired in and hoped, the pipeline runs.
`allure-results/` and `allure-report/` are gitignored, same as `TestResults/`, since they're
build output, not source.

## Design decisions

- **Test → Business → Core, one-way.** Tests read as scenarios (`Login → AddItemToCart →
  OpenCart → Assert`); `IWebDriver`/`RestClient` never appear above the Core/SUT layer. This is
  the thing being graded, so it's the one place I didn't cut corners.
- **Core split into SUT (page objects, API client, DTOs) vs TAS (driver factory, config,
  matchers)** — SUT is saucedemo/jsonplaceholder-specific and gets replaced per project; TAS is
  reusable plumbing and travels with the team.
- **Config over hardcoding.** `Ui`/`Api` settings are POCOs bound from `appsettings.json` via
  `Microsoft.Extensions.Configuration`, with an env-named overlay file and an environment-variable
  override on top — chosen over a hand-rolled JSON reader because the override precedence (env var
  > overlay file > base file) is exactly what a real CI secret store needs, for free.
- **Assumption:** the saucedemo `standard_user` / `secret_sauce` credentials are the vendor's
  public demo login, not a secret — they live in `appsettings.json` as a default. A real secret
  would never be committed; it would come only from `QA_Ui__StandardPassword` env var / CI secret
  store, and `appsettings.json` would hold a placeholder or nothing.
- **Allure + Reqnroll without touching generated code.** Reqnroll generates the test fixture
  (`ShoppingCartFeature`) from the `.feature` file at build time, so `[AllureNUnit]` can't be
  hand-added to its declaration. C# merges attributes across partial class declarations, so a
  second, hand-written `partial class ShoppingCartFeature` carries `[AllureNUnit]` instead —
  verified in the actual Allure output (`allure-results/*-result.json`), not assumed.
- **Skipped for time:** page-object interface abstractions, retry/wait strategy beyond a single
  explicit `WebDriverWait`, and a second UI test — one clean vertical slice per layer over broader
  coverage, per the task's own framing.

## Framework structure

**→ [`FrameworkStructure.md`](FrameworkStructure.md)** — the full answer, in its own file:

| Section | Covers |
|---|---|
| [Layers](FrameworkStructure.md#layers) | The layers and what each one owns |
| [Driver lifecycle, browser config, and where it runs](FrameworkStructure.md#driver-lifecycle-browser-config-and-where-it-runs) | Where driver lifecycle/browser config live; running locally, in a container, on a grid |
| [Environment configuration & secrets](FrameworkStructure.md#environment-configuration--secrets) | Where config and secrets live; pointing the suite at another environment |
| [Page objects, step definitions, test data, API clients](FrameworkStructure.md#page-objects-step-definitions-test-data-api-clients--and-what-cant-leak) | Where each belongs and what isn't allowed to leak between them |
| [Independence & parallelism](FrameworkStructure.md#independence--parallelism) | Keeping tests independent and parallel-safe |
| [Failure reporting & triage](FrameworkStructure.md#failure-reporting--triage) | How failures are reported/investigated; test problem vs. product bug |
| [CI integration](FrameworkStructure.md#ci-integration) | How the suite fits into CI and what it may block |
| [Implemented vs. description-only](FrameworkStructure.md#implemented-vs-description-only) | Which parts exist here and which are description only |
| [Improvements I'd make first, in order](FrameworkStructure.md#improvements-id-make-first-in-order) | What I'd add next, in order |

