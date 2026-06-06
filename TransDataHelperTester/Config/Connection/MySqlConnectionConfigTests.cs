using TransDataHelper.Config.Connection;

namespace TransDataHelper.Tests.Config.Connection;

[TestClass]
public class MySqlConnectionConfigTests
{
    [TestMethod]
    public void BuildConnectionString_ShouldReturnValidFormat()
    {
        // Arrange
        var config = new MySqlConnectionConfig
        {
            DataSource = "192.168.1.100",
            Port = "3306",
            Database = "testdb",
            User = "root",
            Password = "123456",
            ConnectionTimeout = "20",
            Charset = "gbk"
        };

        // Act
        var connStr = config.ConnectionString;

        // Assert
        // 使用语义化的 Assert.Contains
        Assert.Contains("Data Source='192.168.1.100'", connStr);
        Assert.Contains("Port=3306", connStr);
        Assert.Contains("Database='testdb'", connStr);
        Assert.Contains("Uid='root'", connStr);
        Assert.Contains("Pwd='123456'", connStr);
        Assert.Contains("Connection Timeout=20", connStr);
        // 验证备忘录中的 Charset=gbk 必须存在
        Assert.Contains("Charset=gbk", connStr);
    }

    [TestMethod]
    public void BuildConnectionString_EmptyUser_ShouldThrowException()
    {
        // Arrange & Act & Assert
        try
        {
            var config = new MySqlConnectionConfig
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
            Assert.AreEqual("MySql 连接必须提供用户名喵。", ex.Message);
        }
    }
}
