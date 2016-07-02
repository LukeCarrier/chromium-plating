// <copyright file="Program.cs" company="Luke Carrier">
//     Copyright &copy; Luke Carrier
// </copyright>

namespace ChromiumPlating
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization.Json;

    /// <summary>
    /// Program class.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Error code: invalid command line.
        /// </summary>
        protected const int ErrorInvalidCommandLine = 0x667;

        /// <summary>
        /// Configuration filename.
        /// </summary>
        protected string ConfigFilename;

        /// <summary>
        /// Configuration parsed from the configuration file.
        /// </summary>
        protected Config Config;

        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            var program = new Program();
            program.Prepare();

            if (args.Length != 1)
            {
                Console.Error.WriteLine(string.Format(
                        "Usage: {0} <URL>",
                        Process.GetCurrentProcess().MainModule.FileName));
                program.DumpConfig();

                Environment.Exit(ErrorInvalidCommandLine);
            }

            program.Run(args[0]);
        }

        /// <summary>
        /// Prepare the program for execution.
        /// </summary>
        public void Prepare()
        {
            ConfigFilename = LocateConfig();

            try {
                Config = ParseConfig(ConfigFilename);
            }
            catch (FileNotFoundException e)
            {
                Console.Error.WriteLine(string.Format(
                        "Error parsing configuration file: {0}",
                        e.Message));
                Console.Error.WriteLine("Continuing with default configuration");
                ConfigFilename = null;
                Config = new Config();
            }
        }

        /// <summary>
        /// Do the roar.
        /// </summary>
        /// <param name="url">The URL to parse and launch.</param>
        public void Run(string url)
        {
            var profile = Config.ResolveUrlProfile(url);

            if (!Config.ProfileIsWhitelisted(profile))
            {
                throw new IndexOutOfRangeException(string.Format(
                        "Profile {0} is not whitelisted", profile));
            }

            ExecuteBrowser(profile, url);
        }

        /// <summary>
        /// Dump configuration from the configuration file.
        /// </summary>
        public void DumpConfig()
        {
            Console.WriteLine(string.Format(
                    "Configuration file: {0}", ConfigFilename));

            Console.WriteLine(string.Format(
                    "{0} profiles whitelisted:", Config.Browser.Profiles.Count));
            foreach (var profile in Config.Browser.Profiles)
            {
                Console.WriteLine(string.Format("  * {0}", profile));
            }

            Console.WriteLine(string.Format(
                    "{0} URL patterns configured:", Config.Urls.Count));
            foreach (var entry in Config.Urls)
            {
                Console.WriteLine(string.Format("  * {0} => {1}", entry.Pattern, entry.Profile));
            }
        }

        /// <summary>
        /// Execute the browser.
        /// </summary>
        /// <param name="profile">The profile in which to launch the URL.</param>
        /// <param name="url">The URL to launch.</param>
        /// <returns>The resulting process.</returns>
        protected Process ExecuteBrowser(string profile, string url)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Config.Browser.Filename,
                    Arguments = string.Format("--profile-directory=\"{0}\" {1}", profile, url),

                    UseShellExecute = false
                }
            };

            process.Start();
            return process;
        }

        /// <summary>
        /// Locate the configuration file.
        /// </summary>
        /// <returns>The path to the configuration file.</returns>
        protected string LocateConfig()
        {
            IConfigLocator configLocator;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                configLocator = new LinuxConfigLocator();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                configLocator = new WindowsConfigLocator();
            }
            else
            {
                throw new PlatformNotSupportedException(
                        "Unable to locate configuration file on this platform");
            }

            return configLocator.GetConfigFilename();
        }

        /// <summary>
        /// Open and parse the configuration file.
        /// </summary>
        /// <param name="filename">The path to the configuration filename.</param>
        /// <returns>The resulting <see cref="ChromiumPlating.Config" /> object.</returns>
        protected Config ParseConfig(string filename)
        {
            var stream = new FileStream(filename, FileMode.Open);
            return ParseConfig(stream);
        }

        /// <summary>
        /// Parse the configuration stream.
        /// </summary>
        /// <param name="stream">The stream containing the JSON configuration.</param>
        /// <returns>The resulting <see cref="ChromiumPlating.Config" /> object.</returns>
        protected Config ParseConfig(Stream stream)
        {
            var serializer = new DataContractJsonSerializer(typeof(Config));
            var result = (Config)serializer.ReadObject(stream);
            stream.Dispose();

            return result;
        }
    }
}
