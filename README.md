# ASP.NET Core Configuration for Google Cloud

Safely manage and store your sensitive ASP.NET Core application configuration using [Google Cloud Storage buckets](https://cloud.google.com/storage/docs/) for applications hosted on App Engine.

## Installation

You can download `Webenable.Extensions.Configuration.GoogleCloud` from [NuGet](https://www.nuget.org/packages/Webenable.Extensions.Configuration.GoogleCloud/):

```
dotnet add package Webenable.Extensions.Configuration.GoogleCloud
```

## Usage
The easiest way to get started is by calling `AddGoogleCloudConfiguration()` on the web host builder, for example:

```cs
public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
    WebHost.CreateDefaultBuilder(args)
        .UseStartup<Startup>()
        .AddGoogleCloudConfiguration();
```

This will detect whether the application is running on App Engine and it will load the configuration from the Cloud Storage bucket. The storage bucket is polled every minute by default to scan for new changes and reload the configuration. This operation will no-op when the application is not running in App Engine, e.g. when running locally.

By default this library will use the following bucket name format: `{Google Cloud Project Pame}-aspnetcore` and the following file name inside the bucket: `appsettings.{Environment Name}.json`. For example: when the application is hosted in a project called `my-foobar` and running in the `Production` environment it will load `appsettings.Production.json` from the `my-foobar-aspnetcore` bucket. The application will use its default Google Cloud credentials to read the data from the storage bucket.

## Overriding defaults

### Reload interval
The default reload interval can be changed by passing a `TimeSpan` parameter to `AddGoogleCloudConfiguration`, e.g.:

```cs
AddGoogleCloudConfiguration(TimeSpan.FromMinutes(5))
```

Pass `false` for the `withReload` or `null` for the reload interval parameter to disable polling for changes and reloading configuration.

### Bucket and file name
Use the `ConfigureAppConfiguration` delegate to override the default bucket and file name by adding the `GoogleConfigurationSource` manually:

```cs
public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
    WebHost.CreateDefaultBuilder(args)
        .UseStartup<Startup>()
        .ConfigureAppConfiguration((ctx, builder) =>
        {
            builder.Add(new GoogleConfigurationSource("my bucket name", "my file name"));
        });
```

## Encryption
Data in Google Cloud Storage buckets is server-side encrypted by default before it's written to disk. It can only be read using the correct credentials which provides a safe way to store application secrets. Please refer to the [documentation](https://cloud.google.com/storage/docs/encryption/) for more information about encryption.