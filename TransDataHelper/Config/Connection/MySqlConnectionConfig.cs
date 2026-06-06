namespace TransDataHelper.Config.Connection;

/// <summary>
/// MySQL 数据库连接配置
/// </summary>
public class MySqlConnectionConfig : DatabaseConnectionConfig
{
    public MySqlConnectionConfig()
    {
        Port = "3306"; // MySQL 默认端口
        Charset = "utf8mb4";
    }

    public string ConnectionTimeout { get; set; } = "10";
    public string CommandTimeout { get; set; } = "30";

    /// <summary>
    /// 字符集设置。默认 utf8mb4，若读取老库乱码请改为 gbk
    /// </summary>
    public string Charset { get; set; }

    public override string ConnectionString
    {
        get
        {
            if (string.IsNullOrWhiteSpace(User))
                throw new InvalidOperationException("MySql 连接必须提供用户名喵。");

            // 按照主人原有的风格拼接，保持单引号包裹字符串值
            var connStr = $@"
                Data Source='{DataSource}';
                Port={Port};
                Database='{Database}';
                Uid='{User}';
                Pwd='{Password}';
                Connection Timeout={ConnectionTimeout};
                Default Command Timeout={CommandTimeout};
                Charset={Charset};"; // 【备忘录.2】解决中文乱码坑

            return connStr.Replace("\r\n", "").Trim(); // 清理换行符和多余空格
        }
    }
}
