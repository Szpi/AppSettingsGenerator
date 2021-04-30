# AppSettingsGenerator
![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)

## Quick start
Configure file from which settings should be generated in .csproj:
<ItemGroup>
    <AdditionalFiles Include="appsettings.json" />
</ItemGroup>

## Configuration Extensions
For every class generated there will be configuration extension generated to IServiceCollection interface. For given configuration
```
{
  "Logging": {
    "LogLevel": {
        "Default": "Information",
        "Microsoft": "Warning",
        "Microsoft.Hosting.Lifetime23": "Information"
    }
  }
}
```
Following extensions method will be generated:
```
public void ConfigureServices(IServiceCollection services)
{
    services.ConfigureAllSections(Configuration);
    services.ConfigureLogging(Configuration);
    services.ConfigureLogLevel(Configuration);
}
```
Which basicly adds proper section to IServiceCollection.

```
public static void ConfigureLogging(this IServiceCollection services, IConfiguration configuration)
{
    services.Configure<Configuration.Generated.Logging>(configuration.GetSection("Logging"));
}
```

ConfigureAllSections adds every sections at once. In our case it is equivalent to call these two lines:
```
 services.ConfigureLogging(Configuration);
 services.ConfigureLogLevel(Configuration);
```

## Diagnostics
 - APG001 Error
  Configuration in .csproj is missing. To add it please follow instructions in [section](#quick-start).
 - APG002 Info
 Invalid identifier detected. If configuration file contains characters that are not valid in terms of C# class name or property name this information is reported.
 Given json property "Microsoft.Hosting.Lifetime": "Information" dot ('.') is not a valid character in property name, therefore it cannot be generated. In that case
 extension method to IConfiguration is generated. To use it Inject Microsoft.Extensions.Configuration.IConfiguration, add Configuration.Generated namespace and invoke
 IConfiguration.GetMicrosoftHostingLifetime().