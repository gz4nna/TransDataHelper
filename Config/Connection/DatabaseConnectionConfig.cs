namespace TransDataHelper.Config.Connection;

/// <summary>
/// 数据库基础连接配置
/// </summary>
public abstract class DatabaseConnectionConfig
{
    // 默认值全部置空或指向安全本地，保护敏感信息
    public string DataSource { get; set; } = "127.0.0.1";
    public string Port { get; set; } = "";
    public string Database { get; set; } = "";
    public string User { get; set; } = "";
    public string Password { get; set; } = "";

    /// <summary>
    /// 子类实现各自的连接串拼接逻辑（只读）
    /// </summary>
    public abstract string ConnectionString { get; }
}
