using System.Data.Common;
using TransDataHelper.Config.Connection;

namespace TransDataHelper.Adapters;

/// <summary>
/// 数据库连接管理基类，封装通用的开关连接逻辑
/// </summary>
/// <typeparam name="TConnection">具体的数据库连接类型 (如 AseConnection)</typeparam>
/// <typeparam name="TConfig">连接配置类型</typeparam>
public abstract class DbConnectionBase<TConnection, TConfig>
    where TConnection : DbConnection, new()
    where TConfig : DatabaseConnectionConfig
{
    protected readonly TConfig _config;
    protected TConnection? _connection;

    protected DbConnectionBase(TConfig config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    /// <summary>
    /// 获取当前活跃的连接实例
    /// </summary>
    public TConnection? CurrentConnection => _connection;

    /// <summary>
    /// 异步打开数据库连接
    /// </summary>
    public virtual async Task OpenAsync()
    {
        if (_connection != null && _connection.State == System.Data.ConnectionState.Open)
        {
            return;
        }

        _connection = new TConnection { ConnectionString = _config.ConnectionString };

        try
        {
            await _connection.OpenAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"数据库连接失败！请检查配置。Data Source: {_config.DataSource}, Database: {_config.Database}", ex);
        }
    }

    /// <summary>
    /// 异步关闭数据库连接
    /// </summary>
    public virtual async Task CloseAsync()
    {
        if (_connection != null)
        {
            if (_connection.State != System.Data.ConnectionState.Closed)
            {
                await _connection.CloseAsync();
            }
            await _connection.DisposeAsync();
            _connection = null;
        }
    }
}
