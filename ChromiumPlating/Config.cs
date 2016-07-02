// <copyright file="Config.cs" company="Luke Carrier">
//     Copyright &copy; Luke Carrier
// </copyright>

namespace ChromiumPlating
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Configuration state.
    /// </summary>
    [DataContract]
    public class Config
    {
        /// <summary>
        /// Browser section.
        /// </summary>
        [DataMember(Name = "browser")]
        public BrowserConfig Browser;

        /// <summary>
        /// URLs section.
        /// </summary>
        [DataMember(Name = "urls")]
        public List<UrlConfig> Urls;

        /// <summary>
        /// Initializes a new instance of the <see cref="Config" /> class.
        /// </summary>
        public Config()
        {
            Browser = new BrowserConfig();
            Urls = new List<UrlConfig>();
        }

        /// <summary>
        /// Is the named profile whitelisted?
        /// </summary>
        /// <param name="profile">The name of the profile.</param>
        /// <returns>True if whitelisted, else false.</returns>
        public bool ProfileIsWhitelisted(string profile)
        {
            return Browser.Profiles.Contains(profile);
        }

        /// <summary>
        /// Resolve the profile for the specified URL.
        /// </summary>
        /// <param name="url">The URL to resolve for the profile.</param>
        /// <returns>The name of the appropriate profile.</returns>
        public string ResolveUrlProfile(string url)
        {
            foreach (var entry in Urls)
            {
                if (Regex.IsMatch(url, entry.Pattern))
                {
                    return entry.Profile;
                }
            }

            return Browser.Profiles[0];
        }
    }

    /// <summary>
    /// Browser configuration section.
    /// </summary>
    [DataContract]
    public class BrowserConfig
    {
        /// <summary>
        /// Name of the default profile.
        /// </summary>
        protected const string DefaultProfile = "Default";

        /// <summary>
        /// Browser executable filename.
        /// </summary>
        [DataMember(Name = "filename")]
        public string Filename;

        /// <summary>
        /// Browser profile directory.
        /// </summary>
        [DataMember(Name = "profileDirectory")]
        public string ProfileDirectory;

        /// <summary>
        /// Whitelisted browser profiles.
        /// </summary>
        [DataMember(Name = "profiles")]
        public List<string> Profiles;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserConfig" /> class.
        /// </summary>
        public BrowserConfig()
        {
            Profiles = new List<string>()
            {
                DefaultProfile
            };
        }
    }

    /// <summary>
    /// URL configuration section.
    /// </summary>
    [DataContract]
    public class UrlConfig
    {
        /// <summary>
        /// Regular expression these URLs match.
        /// </summary>
        [DataMember(Name = "pattern")]
        public string Pattern;

        /// <summary>
        /// The profile in which to launch the URLs in.
        /// </summary>
        [DataMember(Name = "profile")]
        public string Profile;
    }
}
