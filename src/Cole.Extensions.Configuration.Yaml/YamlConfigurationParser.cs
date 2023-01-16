using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Cole.Extensions.Configuration.Yaml.Resource;
using Microsoft.Extensions.Configuration;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

namespace Cole.Extensions.Configuration.Yaml;

internal sealed class YamlConfigurationParser
{
    private readonly Dictionary<string, string?> _data = new Dictionary<string, string?>(StringComparer.Ordinal);

    //configuration标准实现，应该是大小写不敏感的。但是yaml格式却又是大小写敏感
    //private readonly Dictionary<string, string?> _data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

    private readonly Stack<string> _paths = new Stack<string>();

    public static IDictionary<string, string?> Parse(Stream input)
        => new YamlConfigurationParser().ParseCore(input);

    private Dictionary<string, string?> ParseCore(Stream input)
    {
        using var reader = new StreamReader(input, Encoding.UTF8);
        var yaml = new YamlStream();
        yaml.Load(reader);

        if (!yaml.Any() ||
            yaml.Documents[0].RootNode is not YamlMappingNode mapping)
        {
            throw new FormatException(R.Err_InvalidTopLevelElement);
        }

        VisitMappingNode(mapping);

        return _data;
    }

    #region Visiter

    private void VisitNode(YamlNode node)
    {
        switch (node)
        {
            case YamlMappingNode mappingNode:
                VisitMappingNode(mappingNode);
                break;
            case YamlScalarNode scalarNode:
                VisitScalarNode(scalarNode);
                break;
            case YamlSequenceNode sequenceNode:
                VisitSequenceNode(sequenceNode);
                break;
            default:
                throw new YamlException(node.Start, node.End, R.Err_UnknownYamlNodeType);
        }
    }

    private void VisitMappingNode(YamlMappingNode mappingNode)
    {
        foreach (var pairNode in mappingNode.Children)
        {
            string? name = ((YamlScalarNode) pairNode.Key).Value;
            Debug.Assert(name is not null);
            EnterContext(name);
            VisitNode(pairNode.Value);
            ExitContext();
        }
    }

    private void VisitScalarNode(YamlScalarNode scalarNode)
    {
        string key = _paths.Peek();
        if (_data.ContainsKey(key))
        {
            throw new FormatException(string.Format(R.Err_KeyIsDuplicated, key));
        }

        _data[key] = GetValue(scalarNode);

        //处理yaml中的null值
        static string? GetValue(YamlScalarNode yamlValue)
        {
            return yamlValue is {Style: ScalarStyle.Plain, Value: "~" or "null" or "Null" or "NULL"}
                ? null
                : yamlValue.Value;
        }
    }

    private void VisitSequenceNode(YamlSequenceNode sequenceNode)
    {
        for (int i = 0; i < sequenceNode.Children.Count; i++)
        {
            EnterContext(i.ToString());
            VisitNode(sequenceNode.Children[i]);
            ExitContext();
        }
    }

    #endregion

    #region Context

    private void EnterContext(string context) =>
        _paths.Push(_paths.Count > 0 ? _paths.Peek() + ConfigurationPath.KeyDelimiter + context : context);

    private void ExitContext()
    {
        _paths.Pop();
    }

    #endregion
}