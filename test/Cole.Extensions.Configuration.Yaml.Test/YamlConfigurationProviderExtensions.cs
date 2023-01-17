namespace Cole.Extensions.Configuration.Yaml.Test;

public static class YamlConfigurationProviderExtensions
{
    public static string Get(this YamlConfigurationProvider provider, string key)
    {
        if (!provider.TryGet(key, out string value))
        {
            throw new KeyNotFoundException();
        }

        return value;
    }
}