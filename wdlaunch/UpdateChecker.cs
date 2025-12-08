using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WDLaunch
{
    public class UpdateChecker
    {
        // TODO: Change back to main before merging to main branch
        private const string VersionUrl = "https://raw.githubusercontent.com/emuyia/wd-repackaged/dev/version.json";

        public static async Task<UpdateInfo> CheckForUpdates()
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    string json = await client.DownloadStringTaskAsync(VersionUrl);
                    
                    // Simple Regex parsing
                    var versionMatch = Regex.Match(json, "\"version\"\\s*:\\s*\"(.*?)\"");
                    var urlMatch = Regex.Match(json, "\"url\"\\s*:\\s*\"(.*?)\"");

                    if (versionMatch.Success && urlMatch.Success)
                    {
                        return new UpdateInfo
                        {
                            Version = versionMatch.Groups[1].Value,
                            Url = urlMatch.Groups[1].Value
                        };
                    }
                }
            }
            catch
            {
                
            }
            return null;
        }
    }

    public class UpdateInfo
    {
        public string Version { get; set; }
        public string Url { get; set; }
    }
}
