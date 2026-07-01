using Microsoft.Playwright;

namespace MiniProject.Playwright.Tests.Pages;

/// <summary>
/// Page object for https://playwright.dev/ — converted from example.spec.js.
/// </summary>
public class PlaywrightHomePage : BasePage
{
    private readonly string _url;

    public PlaywrightHomePage(IPage page, string url) : base(page)
    {
        _url = url;
    }

    public ILocator GetStartedLink =>
        Page.GetByRole(AriaRole.Link, new() { Name = "Get started" });

    public ILocator InstallationHeading =>
        Page.GetByRole(AriaRole.Heading, new() { Name = "Installation" });

    public Task NavigateAsync() => Page.GotoAsync(_url);

    public Task ClickGetStartedAsync() => GetStartedLink.ClickAsync();
}
