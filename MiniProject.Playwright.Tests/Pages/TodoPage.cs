using Microsoft.Playwright;

namespace MiniProject.Playwright.Tests.Pages;

/// <summary>
/// Page object for the TodoMVC application.
/// </summary>
public class TodoPage : BasePage
{
    private readonly string _url;

    public TodoPage(IPage page, string url) : base(page)
    {
        _url = url;
    }

    // Todo input textbox
    public ILocator TextInput =>
        Page.Locator(".new-todo");

    // Todo list items
    public ILocator TodoListItems =>
        Page.Locator(".todo-list li");

    // Active filter
    public ILocator ActiveFilterLink =>
        Page.GetByRole(AriaRole.Link, new() { Name = "Active" });

    // Navigate
    public async Task NavigateAsync()
    {
        await Page.GotoAsync(_url);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    // Add one todo
    public async Task AddTodoAsync(string text)
    {
        await TextInput.WaitForAsync();

        await TextInput.FillAsync(text);

        await TextInput.PressAsync("Enter");
    }

    // Add multiple todos
    public async Task AddTodosAsync(IEnumerable<string> items)
    {
        foreach (var item in items)
        {
            await AddTodoAsync(item);
        }
    }

    // Complete a todo
    public async Task ToggleTodoAsync(string text)
    {
        await Page
            .Locator("li", new()
            {
                Has = Page.GetByText(text)
            })
            .Locator(".toggle")
            .CheckAsync();
    }

    // Click Active filter
    public async Task FilterActiveAsync()
    {
        await ActiveFilterLink.ClickAsync();
    }

    // Returns a todo locator
    public ILocator GetTodoTextLocator(string text)
    {
        return Page.GetByText(text);
    }
}