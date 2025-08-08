# Particular.Obsoletes

This package is used by the team at [Particular Software](https://particular.net) to manage `Obsolete` attributes in Particular Software C# projects.

The package includes the [`PreObsolete`](https://github.com/Particular/Particular.Obsoletes/blob/main/src/Particular.Obsoletes.Attributes/PreObsoleteAttribute.cs) and [`ObsoleteMetadata`](https://github.com/Particular/Particular.Obsoletes/blob/main/src/Particular.Obsoletes.Attributes/ObsoleteMetadataAttribute.cs) attributes needed to ensure that `Obsolete` attributes are authored properly.

## Usage

Add the following package reference to your csproj:

```xml
<PackageReference Include="Particular.Obsoletes" Version="{package version} PrivateAssets="All" ExcludeAssets="runtime" />
```

After adding this package to a project, an `ObsoleteMetadata` attribute can be added to API using this format:

```csharp
[ObsoleteMetadata(Message = "Message describing why the API is being deprecated", TreatAsErrorFromVersion = "2", RemoveInVersion = "3")]
```

The package contains fixers that can be used to generate the corresponding `Obsolete` attribute that matched the provided metadata.

## Deployment

Tagged versions are automatically pushed to [feedz.io](https://feedz.io/org/particular-software/repository/packages/packages/Particular.Obsoletes).
