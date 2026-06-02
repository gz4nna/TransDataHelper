using TransDataHelper.Config.Option;
namespace TransDataHelper.Config;

/// <summary>
/// 全局配置参数
/// </summary>
public class GlobalConfig
{
    private static readonly Lock _lock = new();

    private static Options _current = new();

    /// <summary>
    /// 获取或设置当前的全局默认配置。
    /// </summary>
    public static Options Current
    {
        get => _current;
        set
        {
            if (value == null) throw new ArgumentNullException(nameof(value), "全局配置不能设置为 null");
            lock (_lock)
            {
                _current = value;
            }
        }
    }

    /// <summary>
    /// 快速修改全局配置的快捷方法。
    /// 传入一个 Action，在内部加锁安全地修改 Current 配置。
    /// 
    /// 示例：
    /// GlobalConfig.Configure(opt => 
    /// {
    ///     opt.BatchSize = 1000;
    ///     opt.Sybase.CommandTimeout = 300;
    /// });
    /// </summary>
    /// <param name="configure">配置修改委托</param>
    public static void Configure(Action<Options> configure)
    {
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        lock (_lock)
        {
            configure(_current);
        }
    }

    /// <summary>
    /// 将全局配置重置为出厂默认状态。
    /// </summary>
    public static void ResetToDefault()
    {
        lock (_lock)
        {
            _current = new Options();
        }
    }
}
