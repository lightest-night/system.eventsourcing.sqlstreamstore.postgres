using System;
using Npgsql;

namespace LightestNight.System.EventSourcing.SqlStreamStore.Postgres
{
    public class PostgresConnection
    {
        private readonly PostgresEventSourcingOptions _options;
    
        public PostgresConnection(PostgresEventSourcingOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }
    
        public NpgsqlConnection Build()
        {
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = _options.Host,
                Port = _options.Port,
                Database = _options.Database,
                Username = _options.Username,
                Password = _options.Password,
                SslMode = _options.SslMode,
                TrustServerCertificate = _options.TrustServerCertificate,
                ServerCompatibilityMode = _options.ServerCompatibilityMode,
                Timeout = _options.ConnectionTimeout,
                CommandTimeout = _options.CommandTimeout,
                Pooling = _options.Pooling,
                MinPoolSize = _options.MinPoolSize,
                MaxPoolSize = _options.MaxPoolSize,
                ConnectionPruningInterval = _options.ConnectionPruningInterval
            };
            
            return new NpgsqlConnection(builder.ConnectionString);
        }
    }
}