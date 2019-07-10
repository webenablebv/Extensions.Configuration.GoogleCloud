using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;

namespace Webenable.Extensions.Configuration.GoogleCloud
{
    internal class GoogleCloudConfigurationProvider : ConfigurationProvider, IDisposable
    {
        private readonly GoogleCloudConfigurationSource _configurationSource;
        private readonly CancellationTokenSource _cancellationToken;
        private Task _pollingTask;
        private DateTime? _lastUpdated;

        public GoogleCloudConfigurationProvider(GoogleCloudConfigurationSource configurationSource)
        {
            _configurationSource = configurationSource;
            _cancellationToken = new CancellationTokenSource();
            _lastUpdated = GetLastUpdatedAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public override void Load() => LoadAsync(reload: false).ConfigureAwait(false).GetAwaiter().GetResult();

        private async Task LoadAsync(bool reload)
        {
            using (var client = StorageClient.Create())
            using (var ms = new MemoryStream())
            {
                try
                {
                    await client.DownloadObjectAsync(_configurationSource.BucketName, _configurationSource.FileName, ms);
                    ms.Position = 0;
                    Data = JsonConfigurationFileParser.Parse(ms);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to load configuration file '{_configurationSource.FileName}' from Google Cloud Storage bucket '{_configurationSource.BucketName}': {ex.Message}", ex);
                }
            }

            if (reload)
            {
                // Trigger reload of the configuration in the system
                OnReload();
            }

            if (_pollingTask == null && _configurationSource.ReloadTimeout.HasValue)
            {
                // Create a background task to poll for file changes in the bucket
                _pollingTask = PollBucketAsync();
            }
        }

        private async Task<DateTime?> GetLastUpdatedAsync()
        {
            try
            {
                using (var client = StorageClient.Create())
                {
                    return (await client.GetObjectAsync(_configurationSource.BucketName, _configurationSource.FileName)).Updated;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to download '{_configurationSource.FileName}' from Google Cloud Storage bucket '{_configurationSource.BucketName}': {ex.Message}", ex);
            }
        }

        private async Task PollBucketAsync()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                await WaitForReload();
                try
                {
                    // Determine whether the file is updated in the bucket and needs reloading
                    var lastUpdated = await GetLastUpdatedAsync();
                    if (lastUpdated != _lastUpdated)
                    {
                        await LoadAsync(reload: true);
                        _lastUpdated = lastUpdated;
                    }
                }
                catch
                {
                    // Ignore errors while reloading
                }
            }
        }

        private async Task WaitForReload() => await Task.Delay(_configurationSource.ReloadTimeout.Value, _cancellationToken.Token);

        public void Dispose()
        {
            _cancellationToken.Cancel();
        }
    }
}
