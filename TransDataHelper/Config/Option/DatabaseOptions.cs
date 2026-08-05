using TransDataHelper.Config.Connection;

namespace TransDataHelper.Config.Option;

/// <summary>
/// 数据库全局配置选项
/// 职责：聚合连接配置、定义全局行为参数（如批处理大小、降级策略）
/// </summary>
public class DatabaseOptions
{
    /// <summary>
    /// MySQL 连接配置
    /// </summary>
    public MySqlConnectionConfig MySql { get; set; }

    /// <summary>
    /// Oracle 连接配置
    /// </summary>
    public OracleConnectionConfig Oracle { get; set; }

    /// <summary>
    /// Sybase 连接配置
    /// </summary>
    public SybaseConnectionConfig Sybase { get; set; }

    /// <summary>
    /// SQL Server 连接配置
    /// </summary>
    public SqlServerConnectionConfig SqlServer { get; set; }

    private int _batchSize = 500;

    /// <summary>
    /// 单次批量操作的数据量上限
    /// </summary>
    public int BatchSize
    {
        get => _batchSize;
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(BatchSize), "BatchSize 必须大于 0 喵！");
            _batchSize = value;
        }
    }

    private string _targetCharset = "gb2312";

    /// <summary>
    /// 目标字符集（用于数据转换时的参考）
    /// </summary>
    public string TargetCharset
    {
        get => _targetCharset;
        set => _targetCharset = string.IsNullOrWhiteSpace(value) ? "gb2312" : value;
    }

    /// <summary>
    /// 批量操作失败时的降级策略
    /// 默认为 None，即失败直接报错，不进行自动降级，以保证数据安全。
    /// </summary>
    public FallbackStrategy FallbackStrategy { get; set; } = FallbackStrategy.None;

    /// <summary>
    /// 构造函数：初始化所有连接配置实例，防止空引用
    /// </summary>
    public DatabaseOptions()
    {
        MySql = new MySqlConnectionConfig();
        Oracle = new OracleConnectionConfig();
        Sybase = new SybaseConnectionConfig();
        SqlServer = new SqlServerConnectionConfig();
    }
}
