using System.Text;

namespace TransDataHelper.Config.Connection;

/// <summary>
/// Oracle 数据库连接配置
/// 需 NuGet 包: Oracle.ManagedDataAccess.Core
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
            // 1. 安全性校验：用户名不能为空
            if (string.IsNullOrWhiteSpace(User))
                throw new InvalidOperationException("Oracle 连接必须提供用户名喵。");

            // 2. 构建目标数据源
            // 优先级：ServiceName > Database
            string targetDb = ServiceName ?? Database;

            if (string.IsNullOrWhiteSpace(DataSource) || string.IsNullOrWhiteSpace(Port) || string.IsNullOrWhiteSpace(targetDb))
            {
                throw new InvalidOperationException("DataSource、Port 和 数据库标识不能为空喵。");
            }

            // 3. 构建 Data Source 部分 (EZConnect 格式: //host:port/service)
            // 这种格式非常接近 JDBC 的 jdbc:oracle:thin:@//host:port/service
            var dataSourcePart = $"//{DataSource}:{Port}/{targetDb}";

            // 4. 构建完整连接字符串
            var sb = new StringBuilder();
            sb.Append($"Data Source={dataSourcePart};");
            sb.Append($"User Id={User};");
            sb.Append($"Password={Password};");

            // 5. 附加参数
            if (int.TryParse(ConnectionTimeout, out int timeoutVal) && timeoutVal > 0)
            {
                sb.Append($"Connection Timeout={timeoutVal};");
            }

            // 为了防止 SQL 语句在绑定变量时出现的字符集问题，强制 Unicode 是个好习惯喵
            // 不过 ODP.NET 默认行为通常较好，这里保持简洁，不加多余参数

            return sb.ToString().TrimEnd(';');
        }
    }
}
