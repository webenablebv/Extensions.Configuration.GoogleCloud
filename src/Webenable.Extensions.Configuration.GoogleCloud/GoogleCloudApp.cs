using System;

namespace Webenable.Extensions.Configuration.GoogleCloud
{
    internal static class GoogleCloudApp
    {
        public static bool IsRunningOnGoogleCloud => Environment.GetEnvironmentVariable("GAE_INSTANCE") != null;

        public static string GoogleCloudProjectId => Environment.GetEnvironmentVariable("GCLOUD_PROJECT");
    }
}
