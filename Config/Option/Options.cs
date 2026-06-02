using TransDataHelper.Config.Connection;

namespace TransDataHelper.Config.Option
{
    public class Options
    {
        public SybaseConnectionConfig SybaseConnection { get; set; } = new();
        public MySqlConnectionConfig MySqlConnection { get; set; } = new();

        /// <summary>
        /// 单次传输最大数据量
        /// </summary>
        public int BatchSize { get; set; } = 500;

        public string TargetCharset { get; set; } = "gb2312";

        /// <summary>
        /// 当批量插入失败时，采取的降级策略。
        /// </summary>
        public FallbackStrategy FallbackStrategy { get; set; } = FallbackStrategy.FallbackToSingleInsertAndIgnoreDuplicate;

        public SybaseOptions Sybase { get; set; } = new();
        public MySqlOptions MySql { get; set; } = new();
    }
}
