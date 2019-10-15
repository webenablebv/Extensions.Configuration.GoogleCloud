using System;
using Microsoft.Extensions.Configuration;

namespace Webenable.Extensions.Configuration.GoogleCloud
{
    internal class GoogleCloudConfigurationSource : IConfigurationSource
    {
        public GoogleCloudConfigurationSource(string bucketName, string fileName) : this(bucketName, fileName, null)
        {
        }

        public GoogleCloudConfigurationSource(string bucketName, string fileName, TimeSpan? reloadTimeout)
        {
            FileName = fileName;
            BucketName = bucketName;
            ReloadTimeout = reloadTimeout;
        }

        public string BucketName { get; }

        public string FileName { get; }

        public TimeSpan? ReloadTimeout { get; }

        public IConfigurationProvider Build(IConfigurationBuilder builder) => new GoogleCloudConfigurationProvider(this);
    }
}
