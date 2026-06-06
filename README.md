# TransDataHelper

**⚠ AI 辅助生成，请仔细甄别**

## 1. 功能用途

`TransDataHelper` 是一个针对多源异构数据库的通用操作类库。其核心目的在于抹平不同底层数据库驱动（SQLite、MySQL、Oracle、Sybase 等）在连接、执行与数据读取上的操作差异，提供一套统一、简洁的 ADO.NET 交互接口。

本类库特别针对遗留系统环境设计，内置了对古老字符集（如单字节库存储中文）、特殊二进制存储格式等极端场景的数据清洗与还原机制。

## 2. 目前实现

### 2.1 基础架构

配置层 (Config)：建立了统一的 `DatabaseConnectionConfig` 基类及四大数据库的独立配置结构体，支持标准连接串的自动构建与校验。

适配器层 (Adapters)：建立了 `DatabaseAdapter` 基类，统一了 ExecuteReader、ExecuteNonQuery 等核心操作契约。

清洗层 (Helpers)：建立了 `DataSanitizer` 静态工具类，专门负责处理遗留系统的脏数据还原，与适配器层解耦。

### 2.2 已完成的适配器

`SqliteAdapter`：完整实现，通过基类验证。

`MySqlAdapter`：完整实现。排除了旧驱动 Bug，强制要求使用 MySqlConnector，并在连接串中显式声明 Charset=utf8 以杜绝默认字符集引发的乱码。

`OracleAdapter`：完整实现（仅限读取）。采用对齐 Kettle 的 ISO-8859-1 透传策略，通过环境变量设置 NLS_LANG，阻止驱动自动进行错误的 UTF8 转换。配合 `DataSanitizer.RestoreGb2312FromIso8859Passthrough()` 方法，可将透传读取的乱码字符串在 C# 端无损还原为 GB2312 中文。

## 3. 重要客制化说明与使用限制

**本类库中的部分设计是基于特定的业务场景与安全考量做出的强制约束，脱离该场景可能显得不合理，请在使用前务必注意：**

Oracle 严禁写入：`OracleAdapter` 中已显式屏蔽并重写了 `ExecuteNonQuery` 方法，调用将直接抛出 `NotSupportedException`。这是因为 Oracle 在本业务场景中作为不可污染的核心主库，任何通过本程序集的写入行为均被禁止。

Oracle 字符集透传假定：Oracle 的透传读取策略基于服务端为单字节字符集（如 US7ASCII 或 WE8ISO8859P1）且以 GBK 编码强行存储中文的前提。如果您的 Oracle 库是标准的 AL32UTF8 库，请勿使用 `DataSanitizer.RestoreGb2312FromIso8859Passthrough` 方法，否则会导致二次乱码。

.NET Core 中文编码依赖：由于需要使用 GB2312 编码进行数据还原，`DataSanitizer` 内部已调用 `Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)`。请确保运行环境中未被其他组件反向注销该提供程序。

## 4. 使用方法

*(占位：待核心适配器全部完成后补充完整的依赖注入、配置绑定及查询示例代码)*

## 5. 待开发

SybaseAdapter

放弃参数化写入

VARBINARY 中文读取