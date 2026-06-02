namespace TransDataHelper.Config
{
    /// <summary>
    /// 批量操作失败时的降级策略
    /// </summary>
    public enum FallbackStrategy
    {
        /// <summary>
        /// 直接抛出异常，中断整个同步流程。
        /// </summary>
        ThrowImmediately,

        /// <summary>
        /// 降级为单条插入，遇到错误继续抛出异常。
        /// </summary>
        FallbackToSingleInsert,

        /// <summary>
        /// 降级为单条插入，静默忽略主键冲突错误，其他错误抛出异常。
        /// </summary>
        FallbackToSingleInsertAndIgnoreDuplicate,

        /// <summary>
        /// 降级为单条插入，忽略所有错误。
        /// </summary>
        FallbackToSingleInsertAndIgnoreAllErrors
    }
}
