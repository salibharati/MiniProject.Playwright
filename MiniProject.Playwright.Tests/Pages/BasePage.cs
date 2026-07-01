using Microsoft.Playwright;

namespace MiniProject.Playwright.Tests.Pages;

/// <summary>
/// Shared base for all page objects. Holds the IPage reference and any
/// behavior every page should inherit (navigation waits, common helpers).
/// </summary>
public abstract class BasePage
{
    protected readonly IPage Page;

    protected BasePage(IPage page)
    {
        Page = page;
    }

    public Task WaitForPageLoadAsync() =>
        Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

    public Task<string> GetTitleAsync() => Page.TitleAsync();
}
