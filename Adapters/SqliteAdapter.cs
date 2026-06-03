using Microsoft.Data.Sqlite;
using System.Data;
using System.Data.Common;
using TransDataHelper.Config.Connection;

namespace TransDataHelper.Adapters;

/// <summary>
/// SQLite 适配器实现
/// </summary>
public class SqliteAdapter : DatabaseAdapter
{
    public SqliteAdapter(SqliteConnectionConfig config) : base(config)
    {
    }

    protected override IDbConnection CreateConnection()
    {
        return new SqliteConnection(_config.ConnectionString);
    }

    public override int ExecuteNonQuery(string sql, params DbParameter[] parameters)
    {
        if (string.IsNullOrWhiteSpace(sql)) throw new ArgumentNullException(nameof(sql));

        using var cmd = BuildCommand(sql, parameters);
        return cmd.ExecuteNonQuery();
    }

    public override IDataReader ExecuteReader(string sql, params DbParameter[] parameters)
    {
        if (string.IsNullOrWhiteSpace(sql)) throw new ArgumentNullException(nameof(sql));

        // Reader 需要保持连接打开，不能在 using 里包裹 Command，但要妥善处理
        // 这里的策略是：由 Adapter 持有 Connection，Reader 依赖 Connection
        // 调用者负责 Dispose Reader
        var cmd = BuildCommand(sql, parameters);
        // CommandBehavior.CloseConnection 会在 Reader 关闭时自动关闭连接吗？
        // 在我们的设计里，Connection 是由 Adapter 管理的，这里我们不完全依赖这个行为，由 Adapter 统一管理生命周期
        return cmd.ExecuteReader(CommandBehavior.Default);
    }

    private SqliteCommand BuildCommand(string sql, DbParameter[] parameters)
    {
        var cmd = (SqliteCommand)Connection.CreateCommand();
        cmd.CommandText = sql;

        // 绑定参数
        if (parameters != null && parameters.Length > 0)
        {
            cmd.Parameters.AddRange(parameters);
        }

        return cmd;
    }
}
