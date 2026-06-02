using AdoNetCore.AseClient;
using TransDataHelper.Config.Connection;

namespace TransDataHelper.Adapters;

public class SybaseDbAdapter(SybaseConnectionConfig config) :
    DbConnectionBase<AseConnection, SybaseConnectionConfig>(config)
{
}
