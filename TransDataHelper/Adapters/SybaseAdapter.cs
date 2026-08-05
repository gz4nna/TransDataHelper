using AdoNetCore.AseClient;
using System.Data;
using System.Data.Common;
using TransDataHelper.Config.Connection;

namespace TransDataHelper.Adapters;

/// <summary>
/// Sybase ASE 数据库适配器
/// 针对遗留 Sybase 系统的特殊情况，执行严格的写入策略（放弃参数化，强制拼接与日期格式化）
/// </summary>
public class SybaseAdapter : DatabaseAdapter
{
    public SybaseAdapter(SybaseConnectionConfig config) : base(config)
    {
    }

    protected override IDbConnection CreateConnection()
    {
        // 直接使用 Config 生成好的连接字符串
        return new AseConnection(_config.ConnectionString);
    }

    /// <summary>
    /// 执行纯文本非查询 SQL 语句
    /// </summary>
    public override int ExecuteNonQuery(string sql, params DbParameter[] parameters)
    {
        if (string.IsNullOrWhiteSpace(sql)) throw new ArgumentNullException(nameof(sql));
        if (parameters != null && parameters.Length > 0)
        {
            throw new NotSupportedException("Sybase 严禁使用参数化查询，请使用纯文本拼接 SQL 喵！");
        }

        using var cmd = BuildCommand(sql, 120);

        return cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// 执行纯文本查询 SQL 语句
    /// </summary>
    public override IDataReader ExecuteReader(string sql, params DbParameter[] parameters)
    {
        if (string.IsNullOrWhiteSpace(sql)) throw new ArgumentNullException(nameof(sql));
        if (parameters != null && parameters.Length > 0)
        {
            throw new NotSupportedException("Sybase 严禁使用参数化查询，请使用纯文本拼接 SQL 喵！");
        }

        var cmd = BuildCommand(sql, 30);

        return cmd.ExecuteReader(CommandBehavior.CloseConnection);
    }

    /// <summary>
    /// 在单个事务内批量执行多条纯文本 SQL 语句。
    /// 由于 Sybase 不支持多行 VALUES 与 ASE 驱动不支持分号分隔的多语句一次执行，
    /// 故在事务内逐条执行，降低多次自动提交开销，并保证整批原子性。
    /// 全部成功则提交，任一失败则回滚。
    /// </summary>
    /// <param name="sqls">同一事务内按顺序执行的 SQL 语句列表</param>
    /// <param name="commandTimeout">单条命令的超时时间（秒），默认 120</param>
    public void ExecuteBatchInTransaction(List<string> sqls, int commandTimeout = 120)
    {
        ArgumentNullException.ThrowIfNull(sqls);
        if (sqls.Count == 0) return;

        var conn = (AseConnection)Connection;
        using var tx = conn.BeginTransaction();

        try
        {
            for (int i = 0; i < sqls.Count; i++)
            {
                var sql = sqls[i];
                if (string.IsNullOrWhiteSpace(sql))
                    throw new ArgumentException($"批量语句中第 {i + 1} 条为空白，无法执行喵。", nameof(sqls));

                using var cmd = BuildCommand(sql, commandTimeout, tx);
                cmd.ExecuteNonQuery();
            }

            tx.Commit();
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    private AseCommand BuildCommand(string sql, int timeout, AseTransaction? tx = null)
    {
        var cmd = (AseCommand)Connection.CreateCommand();
        cmd.CommandText = sql;
        cmd.CommandTimeout = timeout; // 操作超时参考
        if (tx != null)
        {
            cmd.Transaction = tx; // 将命令绑定到指定事务
        }
        return cmd;
    }
}
