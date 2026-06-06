namespace TransDataHelper.Config.Connection;

/// <summary>
/// Sybase (AdoNetCore.AseClient) 数据库连接配置
/// 需 NuGet 包: AdoNetCore.AseClient
/// </summary>
public class SybaseConnectionConfig : DatabaseConnectionConfig
{
    public SybaseConnectionConfig()
    {
        Port = "5000"; // Sybase 默认端口
    }

    public string ConnectionTimeout { get; set; } = "10";
    public string LoginTimeout { get; set; } = "5";

    public override string ConnectionString
    {
        get
        {
            if (string.IsNullOrWhiteSpace(User))
                throw new InvalidOperationException("Sybase 连接必须提供用户名喵。");

            // 保持主人原有的拼接风格
            var connStr = $@"
                Data Source='{DataSource}';
                Port={Port};
                Database='{Database}';
                Uid='{User}';
                Pwd='{Password}';
                Connection Timeout={ConnectionTimeout};
                Login Timeout={LoginTimeout};";

            return connStr.Replace("\r\n", "").Trim();
        }
    }
}
