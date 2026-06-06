using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.Common;
using TransDataHelper.Config.Connection;

namespace TransDataHelper.Adapters;

/// <summary>
/// Oracle 数据库适配器 (纯胶水版，绝不做数据清洗！)
/// </summary>
public class OracleAdapter : DatabaseAdapter
{
    private static bool _nlsConfigured = false;
    private static readonly object _configLock = new object();

    public OracleAdapter(OracleConnectionConfig config) : base(config)
    {
        ConfigureNlsLangForPassthrough();
    }

    private void ConfigureNlsLangForPassthrough()
    {
        if (_nlsConfigured) return;

        lock (_configLock)
        {
            if (_nlsConfigured) return;

            // 🚀 保留环境变量设置，这是连接层面的合法干预，确保透传
            Environment.SetEnvironmentVariable("NLS_LANG", "AMERICAN_AMERICA.WE8ISO8859P1");

            _nlsConfigured = true;
        }
    }

    protected override IDbConnection CreateConnection()
    {
        return new OracleConnection(_config.ConnectionString);
    }

    /// <summary>
    /// 执行非查询语句 (永久禁用)
    /// </summary>
    public override int ExecuteNonQuery(string sql, params DbParameter[] parameters)
    {
        throw new NotSupportedException("Oracle 作为业务主数据源，严禁执行任何写入操作喵！");
    }

    public override IDataReader ExecuteReader(string sql, params DbParameter[] parameters)
    {
        if (string.IsNullOrWhiteSpace(sql)) throw new ArgumentNullException(nameof(sql));
        var cmd = BuildCommand(sql, parameters);
        return cmd.ExecuteReader(CommandBehavior.CloseConnection);
    }

    private OracleCommand BuildCommand(string sql, DbParameter[] parameters)
    {
        var cmd = (OracleCommand)Connection.CreateCommand();
        cmd.CommandText = sql;
        if (parameters != null && parameters.Length > 0)
            cmd.Parameters.AddRange(parameters);
        return cmd;
    }
}
