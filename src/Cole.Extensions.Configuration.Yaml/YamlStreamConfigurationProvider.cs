using System.IO;
using Microsoft.Extensions.Configuration;

namespace Cole.Extensions.Configuration.Yaml;

public class YamlStreamConfigurationProvider : StreamConfigurationProvider
{
    public YamlStreamConfigurationProvider(StreamConfigurationSource source) : base(source)
    {
    }

    public override void Load(Stream stream)
    {
        Data = YamlConfigurationParser.Parse(stream);
    }
}