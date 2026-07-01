using AventStack.ExtentReports;
using System.Collections.Concurrent;
using NUnit.Framework;

namespace MiniProject.Playwright.Tests.Utilities;

public static class ExtentTestContext
{
    private static readonly ConcurrentDictionary<string, ExtentTest> _tests = new();

    private static string CurrentId => TestContext.CurrentContext.Test.ID;

    public static ExtentTest Current
    {
        get
        {
            if (_tests.TryGetValue(CurrentId, out var test))
                return test;

            throw new InvalidOperationException(
                $"ExtentTest not initialized for test ID: {CurrentId}");
        }
        set
        {
            _tests[CurrentId] = value;
        }
    }

    public static bool HasCurrent =>
        _tests.ContainsKey(CurrentId);

    public static void Clear()
    {
        _tests.TryRemove(CurrentId, out _);
    }
}