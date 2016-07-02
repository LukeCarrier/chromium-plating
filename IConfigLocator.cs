// <copyright file="IConfigLocator.cs" company="Luke Carrier">
//     Copyright &copy; Luke Carrier
// </copyright>

namespace ChromiumPlating
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    /// <summary>
    /// Configuration file locator interface.
    /// </summary>
    public interface IConfigLocator
    {
        /// <summary>
        /// Get configuration filename.
        /// </summary>
        /// <returns>The configuration filename.</returns>
        string GetConfigFilename();
    }

    /// <summary>
    /// Linux configuration file locator.
    /// </summary>
    public class LinuxConfigLocator : IConfigLocator
    {
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Documented in interface")]
        public string GetConfigFilename()
        {
            return Path.Combine(
                    Environment.GetEnvironmentVariable("HOME"),
                    ".chromiumplating",
                    "config.json");
        }
    }

    /// <summary>
    /// Windows configuration file locator.
    /// </summary>
    public class WindowsConfigLocator : IConfigLocator
    {
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Documented in interface")]
        public string GetConfigFilename()
        {
            return Path.Combine(
                    Environment.GetEnvironmentVariable("APPDATA"),
                    "ChromiumPlating",
                    "config.json");
        }
    }
}
