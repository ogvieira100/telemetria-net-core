using OpenTelemetry;
using OpenTelemetry.Logs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    public class SqlLogProcessor : BaseProcessor<LogRecord>
    {
        private readonly string _connectionString;

        public SqlLogProcessor(string connectionString)
        {
            _connectionString = connectionString;
        }

        public override void OnEnd(LogRecord logRecord)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
            INSERT INTO Logs (Timestamp, TraceId, LogLevel, Message)
            VALUES (@Timestamp, @TraceId, @LogLevel, @Message)";

            command.Parameters.Add("@Timestamp", SqlDbType.DateTime).Value = logRecord.Timestamp;
            command.Parameters.Add("@TraceId", SqlDbType.NVarChar, 100).Value = logRecord.TraceId.ToString(); // Convert TraceId to string
            command.Parameters.Add("@LogLevel", SqlDbType.NVarChar, 50).Value = logRecord.LogLevel.ToString();
            command.Parameters.Add("@Message", SqlDbType.NVarChar, -1).Value = logRecord.FormattedMessage ?? string.Empty;

            command.ExecuteNonQuery();
        }
    }

}
