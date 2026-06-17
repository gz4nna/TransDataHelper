using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace TransDataHelper.Helpers
{
    public static class SybaseSqlBuilder
    {
        /// <summary>
        /// 构建读取 SQL：将 text 类型的列替换为 CONVERT(VARBINARY, 列名) AS 列名
        /// </summary>
        public static string BuildQuerySql(string rawSql, Dictionary<string, string> columnTypeMaps)
        {
            if (string.IsNullOrWhiteSpace(rawSql))
                throw new ArgumentNullException(nameof(rawSql));
            if (columnTypeMaps == null || !columnTypeMaps.TryGetValue("text", out string? textCols) || string.IsNullOrWhiteSpace(textCols))
                return rawSql; // 没有 text 类型映射，直接返回原 SQL

            var columns = textCols.Split('|', StringSplitOptions.RemoveEmptyEntries);
            string resultSql = rawSql;

            foreach (var col in columns)
            {
                string trimmedCol = col.Trim();
                if (string.IsNullOrEmpty(trimmedCol)) continue;

                // 使用正则单词边界匹配，避免误伤同名表或别名
                // 匹配前边不是点(.)的独立单词，防止替换掉表别名前缀(如 a.name)
                string pattern = $@"(?<![\w.]){Regex.Escape(trimmedCol)}\b";
                string replacement = $"CONVERT(VARBINARY, {trimmedCol}) AS {trimmedCol}";

                resultSql = Regex.Replace(resultSql, pattern, replacement, RegexOptions.IgnoreCase);
            }

            return resultSql;
        }

        /// <summary>
        /// 构建写入 SQL：根据类型映射拼接 INSERT 语句
        /// </summary>
        public static string BuildInsertSql(string tableName, DataTable data, Dictionary<string, string> columnTypeMaps, int batchSize = 50)
        {
            if (string.IsNullOrWhiteSpace(tableName)) throw new ArgumentNullException(nameof(tableName));
            if (data == null || data.Rows.Count == 0) throw new ArgumentNullException(nameof(data));
            if (columnTypeMaps == null) throw new ArgumentNullException(nameof(columnTypeMaps));

            // 预解析类型映射，方便快速查找
            var typeLookup = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var kvp in columnTypeMaps)
            {
                var cols = kvp.Value.Split('|', StringSplitOptions.RemoveEmptyEntries);
                foreach (var c in cols) typeLookup[c.Trim()] = kvp.Key;
            }

            var columnNames = string.Join(", ", data.Columns.Cast<DataColumn>().Select(c => $"[{c.ColumnName}]"));
            var sqlBuilder = new StringBuilder($"INSERT INTO {tableName} ({columnNames}) VALUES ");

            int rowsProcessed = 0;
            foreach (DataRow row in data.Rows)
            {
                if (rowsProcessed >= batchSize) break;

                if (rowsProcessed > 0) sqlBuilder.Append(", ");

                var valuePlaceholders = string.Join(", ", data.Columns.Cast<DataColumn>()
                    .Select(c => GetSqlValue(row[c], c.ColumnName, typeLookup)));

                sqlBuilder.Append($"({valuePlaceholders})");
                rowsProcessed++;
            }

            return sqlBuilder.ToString();
        }

        /// <summary>
        /// 根据类型映射将值转换为 SQL 安全字符串
        /// </summary>
        private static string GetSqlValue(object value, string columnName, Dictionary<string, string> typeLookup)
        {
            if (value == DBNull.Value || value == null) return "NULL";

            // 尝试获取配置的类型，默认按字符串处理以保安全
            typeLookup.TryGetValue(columnName, out string? typeStr);

            if (typeStr != null)
            {
                if (typeStr.Equals("int", StringComparison.OrdinalIgnoreCase))
                {
                    return value.ToString(); // 数值直接返回
                }
                if (typeStr.Equals("date", StringComparison.OrdinalIgnoreCase) && value is DateTime dt)
                {
                    // 落实红线 6：日期强制格式化 YYYYMMDD
                    return $"'{dt:yyyyMMdd}'";
                }
            }

            // 默认：字符串处理，单引号包裹并转义内部单引号
            string strVal = value.ToString()!;
            return $"'{strVal.Replace("'", "''")}'";
        }
    }
}
