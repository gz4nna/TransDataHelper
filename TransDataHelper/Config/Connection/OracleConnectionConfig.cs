using System.Text;

namespace TransDataHelper.Config.Connection;

/// <summary>
/// Oracle 数据库连接配置 (只读透传版)
/// </summary>
public class OracleConnectionConfig : DatabaseConnectionConfig
{
    public OracleConnectionConfig()
    {
        Port = "1521"; // Oracle 默认端口
    }

    /// <summary>
    /// Oracle 服务名。如果为空，则尝试使用 Database 属性代替。
    /// </summary>
    public string? ServiceName { get; set; }

    public string ConnectionTimeout { get; set; } = "15";

    public override string ConnectionString
    {
        get
        {
            if (string.IsNullOrWhiteSpace(User))
                throw new InvalidOperationException("Oracle 连接必须提供用户名喵。");

            string targetDb = ServiceName ?? Database;

            if (string.IsNullOrWhiteSpace(DataSource) || string.IsNullOrWhiteSpace(Port) || string.IsNullOrWhiteSpace(targetDb))
                throw new InvalidOperationException("DataSource、Port 和 数据库标识 不能为空喵。");

            var dataSourcePart = $"//{DataSource}:{Port}/{targetDb}";

            var sb = new StringBuilder();
            sb.Append($"Data Source={dataSourcePart};");
            sb.Append($"User Id={User};");
            sb.Append($"Password={Password};");

            if (int.TryParse(ConnectionTimeout, out int timeoutVal) && timeoutVal > 0)
                sb.Append($"Connection Timeout={timeoutVal};");

            // 🚀 绝不添加任何多余参数，保持极度纯净！
            return sb.ToString().TrimEnd(';');
        }
    }
}
