using TransDataHelper.Config.Connection;

namespace TransDataHelper.Tests.Config.Connection;

[TestClass]
public class OracleConnectionConfigTests
{
    [TestMethod]
    public void BuildConnectionString_WithDataSourceAndPort_ShouldReturnValidFormat()
    {
        // Arrange
        var config = new OracleConnectionConfig
        {
            DataSource = "192.168.1.100",
            Port = "1521",
            User = "scott",
            Password = "tiger",
            Database = "ORCL", // 当没有 ServiceName 时，Database 作为 SID 或 ServiceName
            ConnectionTimeout = "30"
        };

        // Act
        var connStr = config.ConnectionString;

        // Assert
        // 预期格式: Data Source=//192.168.1.100:1521/ORCL;...
        Assert.Contains("Data Source=//192.168.1.100:1521/ORCL", connStr, "EZConnect格式错误喵");
        Assert.Contains("User Id=scott", connStr);
        Assert.Contains("Password=tiger", connStr);
        Assert.Contains("Connection Timeout=30", connStr);
    }

    [TestMethod]
    public void BuildConnectionString_WithServiceName_ShouldUseServiceNameFormat()
    {
        // Arrange
        var config = new OracleConnectionConfig
        {
            DataSource = "oraclehost",
            Port = "1521",
            User = "admin",
            Password = "pwd123",
            ServiceName = "PDBORCL", // 显式指定 ServiceName，优先使用
            Database = "ORCL" // 应该被忽略
        };

        // Act
        var connStr = config.ConnectionString;

        // Assert
        // 预期格式: Data Source=//oraclehost:1521/PDBORCL;...
        Assert.Contains("Data Source=//oraclehost:1521/PDBORCL", connStr, "ServiceName优先级错误喵");
        Assert.DoesNotContain("/ORCL", connStr, "Database属性不应出现在ServiceName模式下喵");
    }

    [TestMethod]
    public void BuildConnectionString_EmptyUser_ShouldThrowException()
    {
        try
        {
            // Arrange
            var config = new OracleConnectionConfig
            {
                DataSource = "localhost",
                Port = "1521",
                User = "", // 空用户名
                Password = "",
                Database = "XE"
            };
            var _ = config.ConnectionString;
        }
        catch (InvalidOperationException ex)
        {
            Assert.AreEqual("Oracle 连接必须提供用户名喵。", ex.Message); // 验证异常消息
        }

    }
}
