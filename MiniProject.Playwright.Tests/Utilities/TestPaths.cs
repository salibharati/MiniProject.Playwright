using System;

namespace MiniProject.Playwright.Tests.Utilities
{
    /// <summary>
    /// Centralized location for all framework folders.
    ///
    /// Local Execution:
    ///     bin\Release\net8.0\TestResults
    ///
    /// Azure DevOps:
    ///     $(Build.ArtifactStagingDirectory)
    /// </summary>
    public static class TestPaths
    {
        /// <summary>
        /// Root folder used for reports, screenshots, videos and traces.
        /// </summary>
        public static readonly string Root =
            Environment.GetEnvironmentVariable("BUILD_ARTIFACTSTAGINGDIRECTORY")
            ?? Path.Combine(AppContext.BaseDirectory, "TestResults");

        /// <summary>
        /// Extent Report folder
        /// </summary>
        public static readonly string Reports =
            Path.Combine(Root, "Reports");

        /// <summary>
        /// Screenshot folder
        /// </summary>
        public static readonly string Screenshots =
            Path.Combine(Root, "Screenshots");

        /// <summary>
        /// Playwright Video folder
        /// </summary>
        public static readonly string Videos =
            Path.Combine(Root, "Videos");

        /// <summary>
        /// Playwright Trace folder
        /// </summary>
        public static readonly string Traces =
            Path.Combine(Root, "Traces");

        /// <summary>
        /// Execution Log folder
        /// </summary>
        public static readonly string Logs =
            Path.Combine(Root, "Logs");

        /// <summary>
        /// Create all framework folders if they do not already exist.
        /// Call once before test execution.
        /// </summary>
        public static void CreateDirectories()
        {
            Directory.CreateDirectory(Root);
            Directory.CreateDirectory(Reports);
            Directory.CreateDirectory(Screenshots);
            Directory.CreateDirectory(Videos);
            Directory.CreateDirectory(Traces);
            Directory.CreateDirectory(Logs);
        }
    }
}
