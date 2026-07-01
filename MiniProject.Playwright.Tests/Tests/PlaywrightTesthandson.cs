using System.Text.RegularExpressions;
using MiniProject.Playwright.Tests.Base;
using MiniProject.Playwright.Tests.Pages;
using NUnit.Framework;
using static Microsoft.Playwright.Assertions;

namespace MiniProject.Playwright.Tests.Tests
{
    [TestFixture]
    public class PlaywrightTesthandson : BaseTest
    {
        [Test]
        public async Task TestPlaywrightDevNavigation()
        {
            var playwrightHomePage = new PlaywrightHomePage(Page, Settings.Urls.PlaywrightDevUrl);
            await playwrightHomePage.NavigateAsync();
            // Verify the title contains "Playwright"
            await Expect(Page).ToHaveTitleAsync(new Regex("Playwright"));
            // Click the "Get started" link
            await playwrightHomePage.ClickGetStartedAsync();
            // Verify that the Installation heading is visible
            await Expect(playwrightHomePage.InstallationHeading).ToBeVisibleAsync();
        }
    }
}
