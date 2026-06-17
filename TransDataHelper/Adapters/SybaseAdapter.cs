using AdoNetCore.AseClient;
using System.Data;
using System.Data.Common;
using TransDataHelper.Config.Connection;

namespace TransDataHelper.Adapters
{
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

            var cmd = (AseCommand)Connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandTimeout = 120; // 批量操作超时参考

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

            var cmd = (AseCommand)Connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandTimeout = 30; // 查询超时参考

            // 伴随连接关闭，不单独包裹 using
            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }
    }
}
