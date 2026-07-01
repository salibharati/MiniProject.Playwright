using AventStack.ExtentReports;
using System.Threading;

namespace MiniProject.Playwright.Tests.Utilities
{
    /// <summary>
    /// Maintains the current ExtentTest instance for each executing test.
    ///
    /// AsyncLocal ensures every test running in parallel has its own
    /// independent ExtentTest instance while remaining safe across
    /// async/await calls.
    /// </summary>
    public static class ExtentTestContext
    {
        private static readonly AsyncLocal<ExtentTest?> _current = new();

        /// <summary>
        /// Gets or sets the ExtentTest associated with the current test execution.
        /// </summary>
        public static ExtentTest Current
        {
            get
            {
                if (_current.Value == null)
                {
                    throw new InvalidOperationException(
                        "ExtentTest has not been initialized for the current test. " +
                        "Ensure ExtentTestContext.Current is assigned during SetUp.");
                }

                return _current.Value;
            }

            set
            {
                _current.Value = value;
            }
        }

        /// <summary>
        /// Clears the current test reference.
        /// Call this during TearDown to prevent memory leaks.
        /// </summary>
        public static void Clear()
        {
            _current.Value = null;
        }
    }
}