using MySqlConnector;
using System.Data;
using System.Data.Common;
using TransDataHelper.Config.Connection;

namespace TransDataHelper.Adapters;

/// <summary>
/// MySQL 数据库适配器
/// </summary>
public class MySqlAdapter : DatabaseAdapter
{
    public MySqlAdapter(MySqlConnectionConfig config) : base(config)
    {
    }

    protected override IDbConnection CreateConnection()
    {
        // 直接使用 Config 生成好的连接字符串
        return new MySqlConnection(_config.ConnectionString);
    }

    public override int ExecuteNonQuery(string sql, params DbParameter[] parameters)
    {
        if (string.IsNullOrWhiteSpace(sql)) throw new ArgumentNullException(nameof(sql));

        using (var cmd = BuildCommand(sql, parameters))
        {
            return cmd.ExecuteNonQuery();
        }
    }

    public override IDataReader ExecuteReader(string sql, params DbParameter[] parameters)
    {
        if (string.IsNullOrWhiteSpace(sql)) throw new ArgumentNullException(nameof(sql));

        var cmd = BuildCommand(sql, parameters);
        // 伴随连接关闭，不单独包裹 using
        return cmd.ExecuteReader(CommandBehavior.CloseConnection);
    }

    private MySqlCommand BuildCommand(string sql, DbParameter[] parameters)
    {
        var cmd = (MySqlCommand)Connection.CreateCommand();
        cmd.CommandText = sql;

        if (parameters != null && parameters.Length > 0)
        {
            cmd.Parameters.AddRange(parameters);
        }

        return cmd;
    }
}
