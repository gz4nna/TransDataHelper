using System.Text;

namespace TransDataHelper.Helpers;

/// <summary>
/// 遗留数据库数据清洗器
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
    /// 还原通过 ISO-8859-1 透传读取的 GBK/GB2312 中文字符串 (适用于 Oracle)
    /// </summary>
    /// <param name="rawString">从数据库读取到的乱码/透传字符串</param>
    /// <returns>还原后的正常中文字符串</returns>
    public static string RestoreGb2312FromIso8859Passthrough(string rawString)
    {
        if (string.IsNullOrEmpty(rawString)) return rawString;

        EnsureEncodingRegistered();
        byte[] rawBytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(rawString);
        return Encoding.GetEncoding("GB2312").GetString(rawBytes);
    }

    /// <summary>
    /// 还原 Sybase VARBINARY 读取的中文并斩断尾部 0x00
    /// 落实红线 4 (读取坑：尾部补位与编码崩溃)
    /// </summary>
    /// <param name="rawBytes">通过 CONVERT(VARBINARY, 列名) 读取到的原始字节数组</param>
    /// <returns>解码后的正常中文字符串</returns>
    public static string RestoreGb2312FromSybaseBinary(byte[] rawBytes)
    {
        // 防御性编程：空值直接返回空字符串
        if (rawBytes == null || rawBytes.Length == 0) return string.Empty;

        // 落实红线 11：确保 .NET Core+ 环境已注册 GB2312 编码提供程序
        EnsureEncodingRegistered();

        int validLength = rawBytes.Length;
        int idx = validLength - 1;
        while (idx >= 0 && (rawBytes[idx] == 0x00 || rawBytes[idx] == 0x20))
        {
            idx--;
        }
        validLength = idx + 1;

        if (validLength == 0)
        {
            return string.Empty;
        }
        else if (validLength >= 0)
        {
            rawBytes = [.. rawBytes.Take(validLength)];
        }

        // 按生产环境验证的 GB2312 编码手动解码
        return Encoding.GetEncoding("GB2312").GetString(rawBytes);
    }

    /// <summary>
    /// 将字符串转换为Hex表示,绕过驱动转码，适用于 Sybase INSERT 语句中直接插入中文
    /// </summary>
    /// <param name="rawString">utf-16 编码的字符串</param>
    /// <returns>十六进制字符串</returns>
    public static string InsertSybaseHex(string rawString)
    {
        if (string.IsNullOrEmpty(rawString)) return rawString;

        EnsureEncodingRegistered();
        byte[] bytes = Encoding.GetEncoding("GB2312").GetBytes(rawString);
        string hexStr = "0x" + BitConverter.ToString(bytes).Replace("-", "");
        return hexStr;
    }
}
