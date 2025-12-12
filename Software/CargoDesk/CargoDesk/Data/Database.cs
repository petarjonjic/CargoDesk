using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;


namespace CargoDesk.Data
{
    public static class Database
    {
        private const string ConnectionString =
            "Host=localhost;Port=5433;Database=skladiste-projekt;Username=postgres;Password=jonkee101;";

        public static NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(ConnectionString);
        }

        public static async Task<NpgsqlConnection> OpenConnectionAsync()
        {
            var conn = GetConnection();
            await conn.OpenAsync();
            return conn;
        }

        public static async Task TestKonekcijeAsync()
        {
            await using var conn = GetConnection();
            await conn.OpenAsync();

        }

    }
}
