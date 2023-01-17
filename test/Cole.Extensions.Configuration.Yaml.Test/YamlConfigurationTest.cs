using System.Globalization;
using System.Reflection;
using System.Text;
using Xunit;

namespace Cole.Extensions.Configuration.Yaml.Test;

public class YamlConfigurationTest
{
    public YamlConfigurationTest()
    {
        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en");
    }

    private YamlConfigurationProvider GetProvider(string content)
    {
        var provider = new YamlConfigurationProvider(new YamlConfigurationSource());
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        provider.Load(stream);
        return provider;
    }

    [Fact]
    public void LoadKeyValuePairsFromValidYaml()
    {
        // Arrange
        string yaml = """
            receipt: Oz-Ware Purchase Invoice
            date: 2007-08-06T00:00:00.0000000
            customer:
              given: Dorothy
              family: null
            items:
            - part_no: A4786
              price: 1.47
            - part_no: E1628
              price: 100.27
            specialDelivery: >-
              Follow the Yellow Brick
              
              Road to the Emerald City.
            """;

        // Act and Assert
        var p = GetProvider(yaml);
        Assert.Equal("Oz-Ware Purchase Invoice", p.Get("receipt"));
        Assert.Equal("Dorothy", p.Get("customer:given"));
        Assert.Null(p.Get("customer:family"));
        Assert.Equal("A4786", p.Get("items:0:part_no"));
        Assert.Equal("100.27", p.Get("items:1:price"));
        Assert.Equal("Follow the Yellow Brick\nRoad to the Emerald City.", p.Get("specialDelivery"));
    }

    [Fact]
    public void Load_ThrowWhenKeyIsDuplicated()
    {
        // Arrange
        string yaml = """
            name: Cole
            Name: Think
            """;

        // Act and Assert
        var ex = Assert.Throws<FormatException>(() => GetProvider(yaml));
        Assert.NotNull(ex.InnerException);
        Assert.StartsWith("A duplicate key", ex.InnerException.Message);
    }

    [Fact]
    public void Load_CanProcessNullValue()
    {
        // Arrange
        string yaml = """
            value1: null
            value2: Null
            value3: NULL
            value4: ~
            value5: NUll
            value6: NULl
            """;

        // Act and Assert
        var p = GetProvider(yaml);
        Assert.Null(p.Get("value1"));
        Assert.Null(p.Get("value2"));
        Assert.Null(p.Get("value3"));
        Assert.Null(p.Get("value4"));
        Assert.Equal("NUll", p.Get("value5"));
        Assert.Equal("NULl", p.Get("value6"));
    }

    [Fact]
    public void Load_CanProcessBlankAndEmptyValue()
    {
        // Arrange
        string yaml = """
            value1:
            value2: ""
            value3: ''
            """;

        // Act and Assert
        var p = GetProvider(yaml);
        Assert.Equal(string.Empty, p.Get("value1"));
        Assert.Equal(string.Empty, p.Get("value2"));
        Assert.Equal(string.Empty, p.Get("value3"));
    }

    [Fact]
    public void Load_ReturnEmptyWhenFileIsEmpty()
    {
        // Arrange
        string yaml = "";

        // Act and Assert
        var p = GetProvider(yaml);
        var propertyInfo = p.GetType().GetProperty("Data",
            BindingFlags.Instance | BindingFlags.NonPublic);
        var data = (Dictionary<string, string>) propertyInfo.GetValue(p);
        Assert.Empty(data);
    }

    [Fact]
    public void Load_IgnoreComments()
    {
        // Arrange
        string yaml = """
            # comment 1
            name: Cole
            #comment 2
            age: 29
            height: 170cm # comment3
            """;

        // Act and Assert
        var p = GetProvider(yaml);
        Assert.Equal("Cole", p.Get("name"));
        Assert.Equal("29", p.Get("age"));
        Assert.Equal("170cm", p.Get("height"));
    }

    [Fact]
    public void Load_ThrowWhenUnexpectedFirstCharInScalarValue()
    {
        // Arrange
        string yaml = """
            name: {Cole
            """;

        // Act and Assert
        Assert.Throws<FormatException>(() => GetProvider(yaml));
    }

    [Fact]
    public void Load_ThrowWhenUnexpectedFirstCharInKeyValue()
    {
        // Arrange
        string yaml = """
            {name: Cole
            """;

        // Act and Assert
        Assert.Throws<FormatException>(() => GetProvider(yaml));
    }

    [Fact]
    public void Load_ThrowWhenUnexpectedEndOfFile()
    {
        // Arrange
        string yaml = """
            name: Cole
            age
            """;

        // Act and Assert
        Assert.Throws<FormatException>(() => GetProvider(yaml));
    }
}