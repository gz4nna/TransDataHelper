using TransDataHelper.Config.Connection;

namespace TransDataHelper.Tests.Config.Connection;

[TestClass]
public class SqlServerConnectionConfigTests
{
    [TestMethod]
    public void BuildConnectionString_ShouldReturnValidFormat()
    {
        // Arrange
        var config = new SqlServerConnectionConfig
        {
            DataSource = "192.168.1.100",
            Port = "1433",
            Database = "testdb",
            User = "sa",
            Password = "123456",
            ConnectionTimeout = "30",
            TrustServerCertificate = "True"
        };

        // Act
        var connStr = config.ConnectionString;

        // Assert
        // SQL Server 采用 Server=主机,端口 的写法
        Assert.Contains("Server=192.168.1.100,1433", connStr);
        Assert.Contains("Database='testdb'", connStr);
        Assert.Contains("User Id='sa'", connStr);
        Assert.Contains("Password='123456'", connStr);
        Assert.Contains("Connection Timeout=30", connStr);
        Assert.Contains("TrustServerCertificate=True", connStr);
    }

    [TestMethod]
    public void BuildConnectionString_EmptyPort_ShouldOmitPortSuffix()
    {
        // Arrange
        var config = new SqlServerConnectionConfig
        {
            DataSource = "mydbserver",
            Port = "", // 端口留空，应直接使用主机名
            Database = "db",
            User = "sa",
            Password = "pwd"
        };

        // Act
        var connStr = config.ConnectionString;

        // Assert
        // 端口为空时不应出现 ",端口" 后缀
        Assert.Contains("Server=mydbserver", connStr);
        Assert.DoesNotContain(",", connStr.Split(';')[0], "端口为空时不应拼接逗号喵");
    }

    [TestMethod]
    public void BuildConnectionString_EmptyUser_ShouldThrowException()
    {
        // Arrange & Act & Assert
        try
        {
            var config = new SqlServerConnectionConfig
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
            Assert.AreEqual("SQL Server 连接必须提供用户名喵。", ex.Message);
        }
    }

    [TestMethod]
    public void BuildConnectionString_EmptyDataSource_ShouldThrowException()
    {
        // Arrange & Act & Assert
        try
        {
            var config = new SqlServerConnectionConfig
            {
                DataSource = "", // 触发异常
                User = "sa",
                Database = "db"
            };
            var _ = config.ConnectionString;
            Assert.Fail("预期抛出异常，但没有抛出喵！");
        }
        catch (InvalidOperationException ex)
        {
            Assert.AreEqual("DataSource 不能为空喵。", ex.Message);
        }
    }
}
