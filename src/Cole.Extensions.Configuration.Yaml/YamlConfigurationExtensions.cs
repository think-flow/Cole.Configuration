using System;
using System.IO;
using Cole.Extensions.Configuration.Yaml;
using Cole.Extensions.Configuration.Yaml.Resource;
using Microsoft.Extensions.FileProviders;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Configuration;

public static class YamlConfigurationExtensions
{
    public static IConfigurationBuilder AddYamlFile(this IConfigurationBuilder builder, string path)
        => AddYamlFile(builder, null, path, false, false);

    public static IConfigurationBuilder AddYamlFile(this IConfigurationBuilder builder, string path, bool optional)
        => AddYamlFile(builder, null, path, optional, false);

    public static IConfigurationBuilder AddYamlFile(this IConfigurationBuilder builder, string path, bool optional, bool reloadOnChange)
        => AddYamlFile(builder, null, path, optional, reloadOnChange);

    public static IConfigurationBuilder AddYamlFile(this IConfigurationBuilder builder, IFileProvider? provider, string path, bool optional, bool reloadOnChange)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentException(R.Err_InvalidFilePath, nameof(path));
        }

        return builder.AddYamlFile(s =>
        {
            s.Path = path;
            s.Optional = optional;
            s.FileProvider = provider;
            s.ReloadOnChange = reloadOnChange;
            s.ResolveFileProvider();
        });
    }

    public static IConfigurationBuilder AddYamlFile(this IConfigurationBuilder builder, Action<YamlConfigurationSource>? configureSource)
        => builder.Add(configureSource);

    public static IConfigurationBuilder AddYamlStream(this IConfigurationBuilder builder, Stream stream)
    {
        if (builder is null) throw new ArgumentNullException(nameof(builder));

        if (stream is null) throw new ArgumentNullException(nameof(stream));

        return builder.Add<YamlStreamConfigurationSource>(s => s.Stream = stream);
    }
}