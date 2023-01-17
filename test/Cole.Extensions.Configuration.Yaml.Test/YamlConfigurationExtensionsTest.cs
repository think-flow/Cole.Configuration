using System.Globalization;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Cole.Extensions.Configuration.Yaml.Test;

public class YamlConfigurationExtensionsTest
{
    public YamlConfigurationExtensionsTest()
    {
        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en");
    }

    [Fact]
    public void AddYamlFile_ThrowIfConfigurationBuilderIsNull()
    {
        // Arrange
        IConfigurationBuilder builder = null;
        string path = string.Empty;

        // Act and Assert
        var ex = Assert.Throws<ArgumentNullException>(() => builder.AddYamlFile(path));
        Assert.Equal("builder", ex.ParamName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void AddYamlFile_ThrowIfFilePathIsNullOrEmpty(string path)
    {
        // Arrange
        IConfigurationBuilder builder = new ConfigurationBuilder();

        // Act and Assert
        var ex = Assert.Throws<ArgumentException>(() => builder.AddYamlFile(path));
        Assert.Equal("path", ex.ParamName);
        Assert.StartsWith("File path must be a non-empty string.", ex.Message);
    }

    [Fact]
    public void AddYamlFile_ThrowIfFileDoesNotExist()
    {
        // Arrange
        IConfigurationBuilder builder = new ConfigurationBuilder();
        string path = "non-existent file.yml";

        // Act and Assert
        Assert.Throws<FileNotFoundException>(() => builder.AddYamlFile(path).Build());
    }

    [Fact]
    public void AddYamlFile_NotThrowIfFileDoesNotExistAndIsOptional()
    {
        // Arrange
        IConfigurationBuilder builder = new ConfigurationBuilder();
        string path = "non-existent file.yml";
        bool optional = true;

        // Act and Assert
        builder.AddYamlFile(path, optional).Build();
    }
}