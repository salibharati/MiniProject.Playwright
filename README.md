# 🎭 Playwright .NET TodoMVC Automation Framework

A robust End-to-End (E2E) test automation project demonstrating modern testing practices using **Playwright for .NET** with **C#** and **NUnit**, converted from the original JavaScript/Playwright Test project against a React application.

## 🚀 Project Overview

This project automates critical user workflows of the [TodoMVC React Application](https://todomvc.com/examples/react/dist/) and the [Playwright documentation site](https://playwright.dev/), showcasing:
- **Page Object Model (POM)** with a shared `BasePage` and a custom `BaseTest` lifecycle
- **Resilient Locator Strategies** using `GetByRole` and `GetByTestId`
- **Config-driven browser execution** via `appsettings.json` (browser, headless, slow-mo)
- **Screenshot-on-failure, retain-on-failure video, and on-failure tracing**, all implemented in `BaseTest`
- **Async/await throughout**, following idiomatic C# naming conventions

## 🛠️ Tech Stack

- **Automation Tool:** [Playwright for .NET](https://playwright.dev/dotnet/)
- **Language:** C# (.NET 8)
- **Test Runner:** NUnit 4
- **Reporting:** NUnit/TRX output plus attached screenshots, videos, and traces under `TestResults/`

## 📂 Solution Structure

```
MiniProject.Playwright.sln
└── MiniProject.Playwright.Tests/
    ├── appsettings.json          # Browser + test behavior configuration
    ├── Base/
    │   └── BaseTest.cs           # Browser/context lifecycle, screenshots, video, tracing
    ├── Config/
    │   └── AppSettings.cs        # Strongly-typed config model
    ├── Pages/
    │   ├── BasePage.cs
    │   ├── PlaywrightHomePage.cs # playwright.dev page object
    │   └── TodoPage.cs           # TodoMVC page object
    ├── Tests/
    │   ├── ExampleTests.cs       # Converted from example.spec.js
    │   └── TodoTests.cs          # Converted from todo.spec.js
    └── Utilities/
        ├── ConfigReader.cs
        └── ScreenshotHelper.cs
```

## 📂 Test Scenarios Covered

**ExampleTests** (from `example.spec.js`):
1. ✅ **Has Title:** Verifies the playwright.dev homepage title contains "Playwright"
2. ✅ **Get Started Link:** Clicks "Get started" and verifies the Installation heading becomes visible

**TodoTests** (from `todo.spec.js`, tagged `[Category("Sanity")]`):
1. ✅ **Task Creation:** Adding multiple Todo items dynamically
2. ✅ **Task Management:** Marking items as completed
3. ✅ **Filtering:** Verifying the "Active" filter
4. ✅ **Verification:** Asserting visibility of remaining todo items

> **Note:** The final assertion in `TodoApp_Sanity` (expected count of 12) is preserved exactly as it was written in the original `todo.spec.js`. See the comment in `TodoTests.cs` for why that number looks inconsistent with a freshly isolated browser session, and verify it against your environment before relying on it.

## 🏃 How to Run

1. **Clone the repository** (or unzip this project).

2. **Restore NuGet packages:**
   ```bash
   dotnet restore
   ```

3. **Install Playwright browser binaries** (required once per machine):
   ```bash
   dotnet build
   pwsh MiniProject.Playwright.Tests/bin/Debug/net8.0/playwright.ps1 install
   ```
   On Linux/macOS without PowerShell installed, use the .NET tool instead:
   ```bash
   dotnet tool install --global Microsoft.Playwright.CLI
   playwright install
   ```

4. **Run all tests:**
   ```bash
   dotnet test
   ```

5. **Run only the Sanity-tagged tests:**
   ```bash
   dotnet test --filter TestCategory=Sanity
   ```

6. **Run headed** (see the browser): set `"Headless": false` in `appsettings.json` (already the default, matching the original `playwright.config.js`).

7. **Find artifacts after a run:**
   - Screenshots (failures only): `MiniProject.Playwright.Tests/TestResults/Screenshots/`
   - Videos (failures only, deleted automatically on pass): `MiniProject.Playwright.Tests/TestResults/Videos/`
   - Traces (failures only — open with `playwright show-trace <file>.zip`): `MiniProject.Playwright.Tests/TestResults/Traces/`

## ⚙️ Configuration

All settings that previously lived in `playwright.config.js` now live in `appsettings.json`:

| JS (playwright.config.js) | C# (appsettings.json) |
|---|---|
| `use.browserName` | `BrowserSettings.BrowserName` |
| `use.launchOptions.headless` | `BrowserSettings.Headless` |
| `use.launchOptions.slowMo` | `BrowserSettings.SlowMo` |
| `use.screenshot: 'only-on-failure'` | `TestSettings.ScreenshotOnFailure` |
| `use.video: 'retain-on-failure'` | `TestSettings.VideoOnFailure` |
| `use.trace: 'on-first-retry'` | `TestSettings.TraceOnFirstRetry` |
| `retries` | `TestSettings.Retries` (informational; wire up via `[Retry]` if needed) |

## 🤝 Contribution

Feel free to fork this project and submit PRs! Open to discussions on improving test patterns.

---
*Converted to Playwright .NET / C# / NUnit from the original JavaScript project by Pratik - SDET / QA Engineer.*
