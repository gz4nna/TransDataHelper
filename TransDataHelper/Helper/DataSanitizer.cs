using System.Text;

namespace TransDataHelper.Helpers;

/// <summary>
/// 遗留数据库数据清洗器
/// 专门处理因字符集不匹配或特殊存储格式导致的乱码与脏数据问题
/// </summary>
public static class DataSanitizer
{
    private static bool _encodingRegistered = false;
    private static readonly object _registerLock = new object();

    /// <summary>
    /// 确保 GB2312 等中文编码已在 .NET 环境中注册（全局只需一次）
    /// </summary>
    private static void EnsureEncodingRegistered()
    {
        if (_encodingRegistered) return;
        lock (_registerLock)
        {
            if (_encodingRegistered) return;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _encodingRegistered = true;
        }
    }

    /// <summary>
    /// 还原通过 ISO-8859-1 透传读取的 GBK/GB2312 中文字符串
    /// 适用场景：Oracle 单字节库透传读取 (对齐 Kettle 策略)
    /// </summary>
    /// <param name="rawString">从数据库读取到的乱码/透传字符串</param>
    /// <returns>还原后的正常中文字符串</returns>
    public static string RestoreGb2312FromIso8859Passthrough(string rawString)
    {
        if (string.IsNullOrEmpty(rawString)) return rawString;

        EnsureEncodingRegistered();

        // 1. 将透传出来的乱码字符串，按 ISO-8859-1 还原回原始字节数组
        byte[] rawBytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(rawString);

        // 2. 将原始字节数组按真实的 GB2312 编码解码成正常的中文
        return Encoding.GetEncoding("GB2312").GetString(rawBytes);
    }

    // 🚀 预留阵地：未来 Sybase 的 VARBINARY 解码与 0x00 斩断将在这里登场！
    // public static string RestoreGb2312FromVarBinary(byte[] rawBytes) { ... }
}
