using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;


namespace MiniProject.Playwright.Tests.Pages
{
    public class playwrighttest:BasePage
    {
        private readonly string _url;
        public playwrighttest(IPage page, string url) : base(page)
        {
            _url = url;
        }
    }
}
