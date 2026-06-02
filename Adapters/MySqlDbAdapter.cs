using MySql.Data.MySqlClient;
using TransDataHelper.Config.Connection;

namespace TransDataHelper.Adapters;

public class MySqlDbAdapter(MySqlConnectionConfig config) :
    DbConnectionBase<MySqlConnection, MySqlConnectionConfig>(config)
{
}
