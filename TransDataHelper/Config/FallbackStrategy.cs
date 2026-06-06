namespace TransDataHelper.Config;

/// <summary>
/// 批量操作失败时的降级策略
/// </summary>
public enum FallbackStrategy
{
    /// <summary>
    /// 无降级策略。批量操作一旦发生错误，立即抛出异常，中断执行。
    /// 这是对数据完整性要求最高场景的默认选择。
    /// </summary>
    None = 0,

    /// <summary>
    /// 降级为逐条插入。
    /// 如果批量插入失败，自动切换为循环单条插入。
    /// 注意：单条插入时如果再次发生错误（如非主键冲突的数据错误），仍然会抛出异常。
    /// </summary>
    FallbackToSingleInsert,

    /// <summary>
    /// 降级为逐条插入，并静默忽略主键冲突（重复数据）错误。
    /// 适用场景：允许数据重复，只求数据能进去的“幂等写入”场景。
    /// 非主键冲突的其他错误仍会抛出异常。
    /// </summary>
    FallbackToSingleInsertAndIgnoreDuplicate,

    /// <summary>
    /// 降级为逐条插入，并忽略所有单条插入时发生的错误。
    /// 适用场景：“尽力而为”的数据同步，例如日志归档或非关键数据的同步。
    /// 警告：使用此策略可能会导致数据丢失且无感知。
    /// </summary>
    FallbackToSingleInsertAndIgnoreAllErrors
}
