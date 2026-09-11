# Framework Structure

Answers the task's "Framework structure" prompt: not everything below is built in this repo — an
hour only stretches to one vertical slice per layer. What's real vs. described is called out
explicitly per section, and summarized in [Implemented vs. description-only](#implemented-vs-description-only)
below.

## Layers

| Layer | Owns | Must not contain |
|---|---|---|
| **Test** (`Tests/`) | NUnit `[Test]` methods for the API, a Reqnroll `.feature` file + step bindings for the UI. Reads as a business scenario either way. | Selectors, HTTP calls, raw `Assert.That` blocks, driver/client references. |
| **Business** (`Business/`) | `UiSteps`, `ApiSteps`, `AssertionSteps` — orchestration, one concern per method. | Driver/HTTP client instantiation, selectors. |
| **Core / SUT** (`Core/Sut/`) | Page objects (one per page), API clients (one per resource), DTOs/entities — everything that knows *this* system. | References to Business or Test. |
| **Core / TAS** (`Core/Tas/`) | Driver factory & lifecycle, config loader, logger, custom matchers, generic helpers — system-agnostic. | Anything specific to saucedemo/jsonplaceholder. |

Dependency direction is one-way: Test → Business → Core. SUT depends only on what TAS exposes
(a driver instance, an HTTP client, a logger) — never TAS internals.

## Driver lifecycle, browser config, and where it runs

`Core/Tas/Driver/DriverFactory` is the single seam between "a test needs a browser" and "how that
browser gets built." Today it always builds a local Chrome instance. To run the same suite in
different places without touching Business or Test:

- **Locally** — as implemented: `ChromeDriver` with Selenium Manager resolving the binary.
- **In a container** — swap the factory to a `RemoteWebDriver` pointed at a
  `selenium/standalone-chrome` container, gated by an env var (`GRID_URL` unset = local, set =
  remote). A `Dockerfile` for the test project itself (build image, run `dotnet test` in CI) is
  described here, not built.
- **On a grid** (Selenium Grid / BrowserStack / Sauce Labs) — same `RemoteWebDriver` branch, just
  a different `GRID_URL` and `DesiredCapabilities`/`ChromeOptions` (browser, version, OS) sourced
  from config instead of hardcoded.

Browser choice, headless flag, window size, and timeouts are already config-driven
(`UiSettings`) rather than literals in the factory, which is what makes the local/container/grid
switch a config change rather than a code change.

## Environment configuration & secrets

Implemented: `ConfigurationProvider` layers `appsettings.json` → `appsettings.{TEST_ENV}.json`
(optional) → environment variables prefixed `QA_` (`__` as the section separator, e.g.
`QA_Ui__BaseUrl`), and binds the result onto typed `UiSettings`/`ApiSettings`. Pointing the suite
at a different environment is `TEST_ENV=staging dotnet test` plus an `appsettings.staging.json`.

For a real project, actual secrets (non-public credentials, API keys) would never sit in any
committed `appsettings.*.json` — only the env-var layer would carry them, sourced from the CI
secret store (GitHub Actions secrets / Azure Key Vault / etc.) and injected as job env vars, so
locally a developer exports them once and CI injects them per run. Nothing in Business or Core/Sut
would change — they only ever see the bound `UiSettings`/`ApiSettings` object.

## Page objects, step definitions, test data, API clients — and what can't leak

- **Page objects** (`Core/Sut/Ui/Pages`) expose page-level actions and queries (`Login(user,
  pass)`, `GetItemNames()`) — never assertions, never raw selectors leaking upward as return
  values.
- **API clients** (`Core/Sut/Api`) expose one method per operation on a resource and return typed
  DTOs — never a raw `RestResponse` leaking business meaning that Business should own.
- **Step classes** (`Business/`) are the only callers of page objects/API clients, and the only
  place that sequences multiple Core calls into a scenario step. Reqnroll step *definitions*
  (`Tests/Ui/AddItemToCartSteps.cs`, `[Binding]`) are a Test-layer concern, not Business — they
  translate Gherkin into calls on `UiSteps`/`AssertionSteps`, same as a plain `[Test]` method
  would, just parsed from a `.feature` file instead of written directly in C#.
- **Test data**: not built here (single hardcoded item name). At scale, this would be a
  `Core/Sut/*/TestData` folder holding builders/factories producing DTOs/fixtures — Business asks
  for "a valid user," not a Test method constructing a POCO inline.
- **What must not leak**: a selector from a page object into a Business step; an assertion from
  Business into Core; an `IWebDriver`/`RestClient` reference into Business or Test; test data
  construction logic into the Test layer.

## Independence & parallelism

Each `UiSteps` instance owns exactly one `IWebDriver`, created in `SetUp` and disposed in
`TearDown` — no shared driver, no shared mutable state between tests. `GetPostTests` shares one
HTTP response across its two assertion tests via `OneTimeSetUp`, but that response is read-only
after creation, so it's safe under NUnit's default parallel-by-fixture execution
(`[Parallelizable(ParallelScope.Fixtures)]` at the assembly level, not yet added here). Test data
would need to be either fixture-local or created-and-torn-down per test (e.g. a fresh cart per
run) to stay parallel-safe once the suite grows past hand-picked read-only data.

## Failure reporting & triage

Implemented: TRX (machine-readable) + optional HTML report via the built-in `dotnet test` logger;
an Allure report with per-test step breakdown (`AllureApi.Step`) so a UI failure shows exactly
which step it happened on; and NUnit's assertion messages built through the `Matchers` helpers
(each failure states which field/element and what was expected vs. actual). See
[README.md → Test report](README.md#test-report) for how to generate and open these.

Not implemented, described only: a screenshot-on-failure hook (`TearDown` capturing a screenshot
when `TestContext.CurrentContext.Result.Outcome` is a failure, attached to the Allure report via
`AllureApi.AddAttachment`); and trend/history across runs (Allure supports a `history` folder
carried between CI runs so flaky tests become visible as flaky instead of being rediscovered each
time — a single local run doesn't have that history yet).

**Test problem vs. product bug**, the way I'd triage without those tools yet: a test-side failure
throws from Core (`NoSuchElementException`, timeout, connection error) — the site/API structure
changed or the environment is unreachable. A product bug is an assertion failure inside
Business/AssertionSteps — the page loaded, the element existed, the response came back 200, but
the *value* was wrong (item missing from cart, wrong field in the contract). Splitting exceptions
(framework/environment) from assertion failures (product behavior) in triage is why matchers throw
descriptive `Assert.That` failures rather than swallowing exceptions into a generic "test failed."

## CI integration

Not built (out of scope for the hour). Described: a GitHub Actions workflow running
`dotnet test --filter "Category=Smoke"` on every PR (fast signal, blocking merge), and the full
suite on a schedule / on merge to main (non-blocking, reported to Slack/dashboard on failure).
Only the smoke tier would be allowed to block a PR — the full UI suite is the kind of thing that
should page someone for triage, not gate a merge, until its flake rate is proven low.

## Implemented vs. description-only

**Implemented in this repo:** Test/Business/Core split as described above; one UI scenario as a
Reqnroll feature file (`AddItemToCart.feature` + `AddItemToCartSteps.cs`, generating the
`ShoppingCartFeature` NUnit fixture); one API test class with two focused assertions
(`GetPostTests`); `DriverFactory` (local Chrome only); one API client
(`JsonPlaceholderApiClient`) and three page objects (`LoginPage`, `InventoryPage`, `CartPage`);
layered config (`appsettings.json` + env overlay + env var override); reusable `Matchers` used
by `AssertionSteps` instead of inline `Assert.That`; `Category` traits (`Smoke`, `Ui`, `Api`) for
CI filtering; an Allure report with step-level breakdown on both the UI and API tests
(`Allure.NUnit`, verified generating real HTML via the Allure CLI, see
[README.md → Allure report](README.md#allure-report)).

**Description only, not built:** container/grid execution; CI workflow; secrets management
beyond the env-var override mechanism; screenshot-on-failure attachments and cross-run Allure
history/trend; test data builders/factories; parallel execution attributes; a second UI test or
second API resource.

## Improvements I'd make first, in order

1. `[Parallelizable]` at the assembly level + a smoke-tier CI workflow that also publishes the
   Allure report as a build artifact — cheapest wins, unlocks fast PR feedback with a report
   attached.
2. Screenshot-on-failure via `AllureApi.AddAttachment` in `UiSteps.TearDown` — biggest
   triage-time improvement for the least code, and it's a natural extension of the Allure wiring
   that's already there.
3. A test-data builder for the UI item and API resource id, so tests stop hardcoding
   `"Sauce Labs Backpack"` / `1` inline.
4. Container/grid execution via `RemoteWebDriver`, once there's more than one browser/OS
   combination worth running.
