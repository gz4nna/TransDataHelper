using SQLitePCL;

namespace TransDataHelper.Config.Connection;

/// <summary>
/// SQLite 数据库连接配置
/// 需 NuGet 包: Microsoft.Data.Sqlite, SQLitePCLRaw.bundle_e_sqlite3
/// </summary>
public class SqliteConnectionConfig : DatabaseConnectionConfig
{
    /// <summary>
    /// 静态构造函数：确保 SQLite 原生库被正确初始化
    /// </summary>
    static SqliteConnectionConfig()
    {
        try
        {
            Batteries.Init();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("SQLite 引擎初始化失败喵，请检查是否安装了 SQLitePCLRaw.bundle_e_sqlite3 包。", ex);
        }
    }

    public SqliteConnectionConfig()
    {
        Port = "";
    }

    public override string ConnectionString
    {
        get
        {
            if (string.IsNullOrWhiteSpace(DataSource))
                throw new InvalidOperationException("SQLite 必须指定 DataSource (文件路径) 喵。");

            // Mode=ReadWriteCreate: 读写模式，如果文件不存在则自动创建
            // 这就解决了“必须先有文件才能 Open，必须 Open 才能 Create”的互斥问题喵
            return $"Data Source={DataSource};Mode=ReadWriteCreate;";
        }
    }
}
