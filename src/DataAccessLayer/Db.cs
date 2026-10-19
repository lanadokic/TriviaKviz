using System.Data.SqlClient;

namespace DataAccessLayer
{
    public sealed class Db
    {
        private readonly string _connectionString;
        public Db(string connectionString) { _connectionString = connectionString; }

        public SqlConnection OpenConnection()
        {
            var conn = new SqlConnection(_connectionString);
            conn.Open();
            return conn;
        }
    }
}
