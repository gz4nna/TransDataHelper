using System.Data;
using System.Data.Common;
using TransDataHelper.Config.Connection;

namespace TransDataHelper.Adapters;

/// <summary>
/// 数据库适配器基类
/// </summary>
public abstract class DatabaseAdapter : IDisposable
{
    protected readonly DatabaseConnectionConfig _config;
    protected IDbConnection? _connection;
    private bool _disposed = false;

    /// <summary>
    /// 构造函数，注入连接配置
    /// </summary>
    protected DatabaseAdapter(DatabaseConnectionConfig config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    /// <summary>
    /// 获取数据库连接。如果连接未打开，则自动打开。
    /// </summary>
    public IDbConnection Connection
    {
        get
        {
            EnsureConnectionOpen();
            return _connection!;
        }
    }

    /// <summary>
    /// 子类实现：根据配置创建具体的 IDbConnection 实例
    /// </summary>
    protected abstract IDbConnection CreateConnection();

    /// <summary>
    /// 确保连接已打开
    /// </summary>
    protected virtual void EnsureConnectionOpen()
    {
        if (_connection == null)
        {
            _connection = CreateConnection();
        }

        if (_connection.State != ConnectionState.Open)
        {
            try
            {
                _connection.Open();
            }
            catch (Exception ex)
            {
                // 捕获连接打开异常并包装，添加上下文信息
                throw new InvalidOperationException($"无法打开数据库连接喵。DataSource: {_config.DataSource}, Database: {_config.Database}", ex);
            }
        }
    }

    /// <summary>
    /// 执行非查询 SQL 语句
    /// </summary>
    public abstract int ExecuteNonQuery(string sql, params DbParameter[] parameters);

    /// <summary>
    /// 执行查询 SQL 语句
    /// </summary>
    public abstract IDataReader ExecuteReader(string sql, params DbParameter[] parameters);

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _connection?.Close();
                _connection?.Dispose();
                _connection = null;
            }
            _disposed = true;
        }
    }
}
