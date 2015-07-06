using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WebAPI.Models
{
    class SqlCommandHelper : IEnumerable<DynamicSqlRow>, IDisposable
    {
        private SqlConnection connection;
        private SqlCommand command;

        public SqlCommandHelper(string connectionString, System.Data.CommandType commandType, string commandText, params SqlParameter[] parameters)
        {
            connection = new SqlConnection(connectionString);
            command = new SqlCommand
            {
                CommandText = commandText,
                CommandType = commandType,
                Connection = connection
            };

            command.Parameters.AddRange(parameters);
        }

        public IEnumerable<DynamicSqlRow> getRows()
        {
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    yield return new DynamicSqlRow(reader);
                }
            }
        }

        public IEnumerator<DynamicSqlRow> GetEnumerator()
        {
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    yield return new DynamicSqlRow(reader);
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Dispose()
        {
            command.Dispose();
            connection.Dispose();
        }
    }
}