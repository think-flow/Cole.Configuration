using System;
using System.IO;
using Cole.Extensions.Configuration.Yaml.Resource;
using Microsoft.Extensions.Configuration;

namespace Cole.Extensions.Configuration.Yaml;

public class YamlConfigurationProvider : FileConfigurationProvider
{
    public YamlConfigurationProvider(FileConfigurationSource source) : base(source)
    {
    }

    public override void Load(Stream stream)
    {
        try
        {
            Data = YamlConfigurationParser.Parse(stream);
        }
        catch (Exception e)
        {
            throw new FormatException(string.Format(R.Err_YamlParseError, Source.Path), e);
        }
    }
}