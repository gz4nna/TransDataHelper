using TransDataHelper.Config;
using TransDataHelper.Config.Option;

namespace TransDataHelper.Tests.Config.Option;

[TestClass]
public class DatabaseOptionsTests
{
    [TestMethod]
    public void Constructor_DefaultValues_ShouldBeSafe()
    {
        // Arrange & Act
        var options = new DatabaseOptions();

        // Assert
        Assert.IsNotNull(options.MySql);
        Assert.IsNotNull(options.Oracle);
        Assert.IsNotNull(options.Sybase);

        // 默认 BatchSize 应该是一个合理的值
        Assert.IsGreaterThan(0, options.BatchSize, "BatchSize 必须大于 0 喵");

        // 默认策略必须是 None，不能默认静默丢数据！
        Assert.AreEqual(FallbackStrategy.None, options.FallbackStrategy);
    }

    [TestMethod]
    public void BatchSize_SetToNegative_ShouldThrowException()
    {
        // Arrange
        var options = new DatabaseOptions();

        // Act & Assert
        try
        {
            options.BatchSize = -1;
            Assert.Fail("设置负数的 BatchSize 应该抛出异常喵！");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Assert.Contains("BatchSize", ex.Message);
        }
    }
}
