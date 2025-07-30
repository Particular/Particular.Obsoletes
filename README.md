# Particular.Obsoletes

This package is used by the team at [Particular Software](https://particular.net) to support the deprecation process of APIs in a component.

## Usage
After adding this package to the .csproj, an ObsoleteMetadata attribute can be added to API using this format: 

```
[ObsoleteMetadata(
        Message = "The DataBus feature has been released as a dedicated package, 'NServiceBus.ClaimCheck'",
        RemoveInVersion = "11",
        TreatAsErrorFromVersion = "10")]
[Obsolete("The DataBus feature has been released as a dedicated package, 'NServiceBus.ClaimCheck'. Will be removed in version 11.0.0.", true)]
public static class ConfigureFileShareDataBus
```

The package also contains fixers, which will provide helpful solutions if the Obsolete attribute doesn't match with the ObsoleteMetadata attribute

## Deployment

Tagged versions are automatically pushed to [feedz.io](https://feedz.io/org/particular-software/repository/packages/packages/Particular.Analyzers). After validating new versions, the package should be promoted to production by pushing the package to NuGet using the feedz.io push upstream feature.
