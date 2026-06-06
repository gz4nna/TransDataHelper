using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.Common;
using System.Text;
using TransDataHelper.Config.Connection;

namespace TransDataHelper.Adapters;

/// <summary>
/// Oracle 数据库适配器 (只读透传版)
/// 严禁执行任何写入操作，读取时对齐 Kettle 的 ISO-8859-1 透传策略，保留原始字节供业务层解码。
/// </summary>
public class OracleAdapter : DatabaseAdapter
{
    private static bool _nlsConfigured = false;
    private static readonly object _configLock = new object();

    public OracleAdapter(OracleConnectionConfig config) : base(config)
    {
        ConfigureNlsLangForPassthrough();
    }

    /// <summary>
    /// 配置 NLS_LANG 以实现字节透传
    /// 对齐 Kettle 策略：告知驱动服务端是 ISO-8859-1，阻止驱动自动进行 UTF8 转换，
    /// 从而将包含 GBK 粘合字节的原始流无损带到 C# 端。
    /// </summary>
    private void ConfigureNlsLangForPassthrough()
    {
        if (_nlsConfigured) return;

        lock (_configLock)
        {
            if (_nlsConfigured) return;

            // 🚀 终极修复：对齐 Kettle，声明客户端字符集为 ISO-8859-1
            // ODP.NET Core 启动时会读取此环境变量，从而阻止它自动进行 UTF8 转换
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
    /// <exception cref="NotSupportedException">Oracle 作为业务主数据源，严禁通过本程序集进行写入操作。</exception>
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

    // 🚀 提供专用的字节提取方法，供业务层对乱码字符串进行 GBK 还原
    // 因为我们用 ISO-8859-1 读出来的中文会变成乱码字符串，需要这个方法还原
    public static string ConvertPassthroughStringToGb2312(string rawString)
    {
        if (string.IsNullOrEmpty(rawString)) return rawString;

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        // 1. 将透传出来的乱码字符串，按 ISO-8859-1 还原回原始字节数组
        byte[] rawBytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(rawString);

        // 2. 将原始字节数组按真实的 GBK 编码解码成正常的中文
        return Encoding.GetEncoding("GB2312").GetString(rawBytes);
    }
}
