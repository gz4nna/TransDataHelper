namespace TransDataHelper.Config.Connection;

/// <summary>
/// SQL Server (Microsoft.Data.SqlClient) 数据库连接配置
/// 需 NuGet 包: Microsoft.Data.SqlClient
/// </summary>
public class SqlServerConnectionConfig : DatabaseConnectionConfig
{
    public SqlServerConnectionConfig()
    {
        Port = "1433"; // SQL Server 默认端口
    }

    public string ConnectionTimeout { get; set; } = "15";
    public string TrustServerCertificate { get; set; } = "True";
    /// <summary>
    /// 是否启用 TLS 加密。
    /// 默认 False：内网无隔离场景下与老版本 SQL Server（仅支持 TLS 1.0）直连，
    /// 避免强制加密导致的 TLS 1.0 协商警告与握手失败。
    /// 若目标库确实需要加密，可在插件前端参数中改为 True（SqlServerEncrypt）。
    /// </summary>
    public string Encrypt { get; set; } = "False";

    public override string ConnectionString
    {
        get
        {
            if (string.IsNullOrWhiteSpace(User))
                throw new InvalidOperationException("SQL Server 连接必须提供用户名喵。");

            if (string.IsNullOrWhiteSpace(DataSource))
                throw new InvalidOperationException("DataSource 不能为空喵。");

            // 保持主人原有的拼接风格，兼容默认端口与自定义端口
            var serverPart = string.IsNullOrWhiteSpace(Port)
                ? DataSource
                : $"{DataSource},{Port}";

            var connStr = $@"
                Server={serverPart};
                Database='{Database}';
                User Id='{User}';
                Password='{Password}';
                Connection Timeout={ConnectionTimeout};
                Encrypt={Encrypt};
                TrustServerCertificate={TrustServerCertificate};";

            return connStr.Replace("\r\n", "").Trim();
        }
    }
}
