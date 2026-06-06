using TransDataHelper.Config.Connection;

namespace TransDataHelper.Tests.Config.Connection;

[TestClass]
public class SybaseConnectionConfigTests
{
    [TestMethod]
    public void BuildConnectionString_ShouldReturnValidFormat()
    {
        // Arrange
        var config = new SybaseConnectionConfig
        {
            DataSource = "192.168.1.200",
            Port = "5000",
            Database = "misdb",
            User = "sa",
            Password = "password",
            ConnectionTimeout = "15"
        };

        // Act
        var connStr = config.ConnectionString;

        // Assert
        Assert.Contains("Data Source='192.168.1.200'", connStr);
        Assert.Contains("Port=5000", connStr);
        Assert.Contains("Database='misdb'", connStr);
        Assert.Contains("Uid='sa'", connStr);
        Assert.Contains("Pwd='password'", connStr);
        Assert.Contains("Connection Timeout=15", connStr);
        Assert.Contains("Login Timeout=5", connStr);
    }

    [TestMethod]
    public void BuildConnectionString_EmptyUser_ShouldThrowException()
    {
        // Arrange & Act & Assert
        try
        {
            var config = new SybaseConnectionConfig
            {
                DataSource = "localhost",
                User = "", // 触发异常
                Database = "db"
            };
            var _ = config.ConnectionString;
            Assert.Fail("预期抛出异常，但没有抛出喵！");
        }
        catch (InvalidOperationException ex)
        {
            Assert.AreEqual("Sybase 连接必须提供用户名喵。", ex.Message);
        }
    }
}
