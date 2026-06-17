using TransDataHelper.Helpers;

namespace TransDataHelperTester.Helper;

[TestClass]
public class DataSanitizerTests
{
    [TestMethod]
    public void DataSanitizer_SybaseBinary_ShouldCutOffZeroAndDecodeGb2312()
    {
        // 安排
        // 模拟 Sybase CONVERT(VARBINARY) 读取到的中文 "测试" 加上尾部的 \0
        // GB2312 中 "测试" 的字节为: 178, 226, 202, 212
        byte[] rawBytes = [178, 226, 202, 212, 0, 0, 0, 0];

        // 行动
        // 落实红线 4：先截取 0x00 前的有效字节，再用 GB2312 解码
        string result = DataSanitizer.RestoreGb2312FromSybaseBinary(rawBytes);

        // 断言
        Assert.AreEqual("测试", result, "应当正确斩断 0x00 并解码为中文喵。");
    }

    [TestMethod]
    public void DataSanitizer_SybaseBinary_AllZeros_ShouldReturnEmpty()
    {
        // 安排
        byte[] rawBytes = [0, 0, 0];

        // 行动
        string result = DataSanitizer.RestoreGb2312FromSybaseBinary(rawBytes);

        // 断言
        Assert.AreEqual(string.Empty, result, "全零数组应安全返回空字符串，不引发异常喵。");
    }
}
