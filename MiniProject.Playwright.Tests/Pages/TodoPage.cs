using Microsoft.Playwright;

namespace MiniProject.Playwright.Tests.Pages;

/// <summary>
/// Page object for the TodoMVC React demo app — converted from todo.spec.js.
/// </summary>
public class TodoPage : BasePage
{
    private readonly string _url;

    public TodoPage(IPage page, string url) : base(page)
    {
        _url = url;
    }

    public ILocator TextInput => Page.GetByTestId("text-input");

    public ILocator TodoListItems => Page.Locator(".todo-list li");

    public ILocator ActiveFilterLink =>
        Page.GetByRole(AriaRole.Link, new() { Name = "Active" });

    public Task NavigateAsync() => Page.GotoAsync(_url);

    public async Task AddTodoAsync(string text)
    {
        await TextInput.ClickAsync();
        await TextInput.FillAsync(text);
        await TextInput.PressAsync("Enter");
    }

    public async Task AddTodosAsync(IEnumerable<string> items)
    {
        foreach (var item in items)
        {
            await AddTodoAsync(item);
        }
    }

    public Task ToggleTodoAsync(string text) =>
        Page.GetByRole(AriaRole.Listitem)
            .Filter(new() { HasText = text })
            .GetByTestId("todo-item-toggle")
            .CheckAsync();

    public Task FilterActiveAsync() => ActiveFilterLink.ClickAsync();

    public ILocator GetTodoTextLocator(string text) => Page.GetByText(text);
}
