using MiniProject.Playwright.Tests.Utilities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject.Playwright.Tests
{
    [SetUpFixture]
    public class GlobalSetupFixture
    {
        [OneTimeSetUp]
        public void Setup()
        {
            _ = ExtentReportManager.Instance;
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            ExtentReportManager.Flush();
        }
    }
}
