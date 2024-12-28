using OpenTelemetry;
using OpenTelemetry.Logs;
using System;
using System.Collections.Generic;
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

            command.Parameters.AddWithValue("@Timestamp", logRecord.Timestamp);
            command.Parameters.AddWithValue("@TraceId", logRecord.TraceId );
            command.Parameters.AddWithValue("@LogLevel", logRecord.LogLevel.ToString());
            command.Parameters.AddWithValue("@Message", logRecord.FormattedMessage ?? string.Empty);

            command.ExecuteNonQuery();
        }
    }

}
