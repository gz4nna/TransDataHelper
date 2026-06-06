using Microsoft.Data.Sqlite;
using TransDataHelper.Adapters;
using TransDataHelper.Config.Connection;

namespace TransDataHelper.Tests.Adapters;

[TestClass]
public class SqliteAdapterTests
{
    private const string TestDbFile = "test_integration.db";
    private SqliteConnectionConfig _config = null!;
    private SqliteAdapter _adapter = null!;

    [TestInitialize]
    public void Setup()
    {
        // 每个测试前，清理旧文件，准备新环境
        if (System.IO.File.Exists(TestDbFile))
        {
            System.IO.File.Delete(TestDbFile);
        }

        // SQLite 的 Data Source 就是文件路径喵
        _config = new SqliteConnectionConfig
        {
            DataSource = TestDbFile,
            Database = "Test",
            // SQLite 不需要 User/Password
        };

        _adapter = new SqliteAdapter(_config);
    }

    //[TestCleanup]
    //public void Cleanup()
    //{
    //    _adapter?.Dispose();
    //    if (System.IO.File.Exists(TestDbFile))
    //    {
    //        System.IO.File.Delete(TestDbFile);
    //    }
    //}

    [TestMethod]
    public void ExecuteNonQuery_CreateTable_ShouldWork()
    {
        // Act
        var sql = "CREATE TABLE Test (Id INTEGER PRIMARY KEY, Name TEXT);";
        var rows = _adapter.ExecuteNonQuery(sql);

        // Assert
        // DDL 语句通常返回 -1，某些驱动可能返回 0
        Assert.IsTrue(rows == -1 || rows == 0, "创建表失败喵");
    }

    [TestMethod]
    public void ExecuteNonQuery_And_ExecuteReader_ShouldFlow()
    {
        // Arrange: 建表
        _adapter.ExecuteNonQuery("CREATE TABLE Cats (Id INTEGER PRIMARY KEY, Name TEXT);");

        // Act: 插入数据
        var insertSql = "INSERT INTO Cats (Name) VALUES (@Name);";
        var param = new SqliteParameter("@Name", "Mikey");
        _adapter.ExecuteNonQuery(insertSql, param);

        // Assert: 查询数据
        var selectSql = "SELECT Name FROM Cats WHERE Id = @Id;";
        var idParam = new SqliteParameter("@Id", 1);
        using var reader = _adapter.ExecuteReader(selectSql, idParam);

        Assert.IsTrue(reader.Read(), "应该读出一行数据喵");
        Assert.AreEqual("Mikey", reader["Name"]);
    }
}
