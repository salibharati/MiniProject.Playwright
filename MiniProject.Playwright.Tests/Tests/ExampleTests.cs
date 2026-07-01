using System.Text.RegularExpressions;
using MiniProject.Playwright.Tests.Base;
using MiniProject.Playwright.Tests.Pages;
using NUnit.Framework;
using static Microsoft.Playwright.Assertions;

namespace MiniProject.Playwright.Tests.Tests;

/// <summary>
/// Converted from example.spec.js. Both original scenarios are preserved:
///   1. "has title"        -> HasTitle_ContainsPlaywright
///   2. "get started link" -> GetStartedLink_NavigatesToInstallation
/// </summary>
[TestFixture]
public class ExampleTests : BaseTest
{
    private PlaywrightHomePage _homePage = null!;

    [SetUp]
    public void InitializePage()
    {
        _homePage = new PlaywrightHomePage(Page, Settings.Urls.PlaywrightDevUrl);
    }

    [Test]
    public async Task HasTitle_ContainsPlaywright()
    {
        await _homePage.NavigateAsync();

        // Original: await expect(page).toHaveTitle(/Playwright/);
        await Expect(Page).ToHaveTitleAsync(new Regex("Playwright"));
    }

    [Test]
    public async Task GetStartedLink_NavigatesToInstallation()
    {
        await _homePage.NavigateAsync();
        await _homePage.ClickGetStartedAsync();

        // Original: await expect(page.getByRole('heading', { name: 'Installation' })).toBeVisible();
        await Expect(_homePage.InstallationHeading).ToBeVisibleAsync();
    }
}
