using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Webenable.Extensions.Configuration.GoogleCloud
{
    /// <summary>
    /// Extensions for configuration Google Cloud application configuration.
    /// </summary>
    public static class GoogleConfigurationExtensions
    {
        /// <summary>
        /// Adds application configuration from Google Cloud Storage from the default location with a default reload delay of 1 minute.
        /// </summary>
        public static IWebHostBuilder AddGoogleCloudConfiguration(this IWebHostBuilder builder) =>
            AddGoogleCloudConfiguration(builder, withReload: true);

        /// <summary>
        /// Adds application configuration from Google Cloud Storage from the default location with an optional reload delay of 1 minute.
        /// </summary>
        /// <param name="builder">The web host builder.</param>
        /// <param name="withReload">Specifies whether to automatically reload configuration every minute.</param>
        public static IWebHostBuilder AddGoogleCloudConfiguration(this IWebHostBuilder builder, bool withReload) =>
            AddGoogleCloudConfiguration(builder, withReload ? TimeSpan.FromMinutes(1) : (TimeSpan?)null);

        /// <summary>
        /// Adds application configuration from Google Cloud Storage from the default location with the specified reload delay.
        /// </summary>
        /// <param name="builder">The web host builder.</param>
        /// <param name="reloadDelay">The amount of delay for reloading the configuration. Specify null to disable reloading.</param>
        public static IWebHostBuilder AddGoogleCloudConfiguration(this IWebHostBuilder builder, TimeSpan? reloadDelay)
        {
            if (!GoogleCloudApp.IsRunningOnGoogleCloud)
            {
                return builder;
            }

            builder.ConfigureAppConfiguration((ctx, configBuilder) =>
            {
                var envName = ctx.HostingEnvironment.EnvironmentName;
                configBuilder.AddGoogleCloud($"{GoogleCloudApp.GoogleCloudProjectId}-aspnetcore", $"appsettings.{envName}.json", reloadDelay);
            });
            return builder;
        }

        /// <summary>
        /// Adds application configurato=ion from Google Cloud Storage from the specified bucket and file name.
        /// </summary>
        /// <param name="builder">The configuration builder.</param>
        /// <param name="bucketName">The name of the bucket in Google Cloud Storage.</param>
        /// <param name="fileName">The name of the file in the Google Cloud Storage bucket.</param>
        public static void AddGoogleCloud(this IConfigurationBuilder builder, string bucketName, string fileName) =>
            AddGoogleCloud(builder, bucketName, fileName, reloadDelay: null);

        /// <summary>
        /// Adds application configurato=ion from Google Cloud Storage from the specified bucket and file name.
        /// </summary>
        /// <param name="builder">The configuration builder.</param>
        /// <param name="bucketName">The name of the bucket in Google Cloud Storage.</param>
        /// <param name="fileName">The name of the file in the Google Cloud Storage bucket.</param>
        /// <param name="reloadDelay">The amount of delay for reloading the configuration. Specify null to disable reloading.</param>
        public static void AddGoogleCloud(this IConfigurationBuilder builder, string bucketName, string fileName, TimeSpan? reloadDelay)
        {
            builder.Add(new GoogleCloudConfigurationSource(bucketName, fileName, reloadDelay));
        }
    }
}
