using LightestNight.System.Data.Postgres;
using Nelibur.ObjectMapper;
using Npgsql;

namespace LightestNight.System.EventSourcing.SqlStreamStore.Postgres
{
    public class PostgresEventSourcingOptions : EventSourcingOptions
    {
        static PostgresEventSourcingOptions()
        {
            TinyMapper.Bind<PostgresEventSourcingOptions, PostgresOptions>();
        }
        
        /// <summary>
        /// Whether to create the database schema if it doesn't already exist
        /// </summary>
        public bool CreateSchemaIfNotExists { get; set; } = true;

        /// <summary>
        /// The Postgres database server host
        /// </summary>
        public string Host { get; set; } = string.Empty;

        /// <summary>
        /// The Postgres database server port
        /// </summary>
        public int Port { get; set; } = 5432;

        /// <summary>
        /// The Postgres database name
        /// </summary>
        public string Database { get; set; } = string.Empty;

        /// <summary>
        /// The Postgres database username
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// The Postgres database password 
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// The <see cref="SslMode" /> to use in the Postgres connection
        /// </summary>
        public SslMode SslMode { get; set; } = SslMode.Prefer;
        
        /// <summary>
        /// Whether to trust the server certificate
        /// </summary>
        public bool TrustServerCertificate { get; set; }

        /// <summary>
        /// The <see cref="ServerCompatibilityMode" /> to use in the Postgres connection
        /// </summary>
        public ServerCompatibilityMode ServerCompatibilityMode { get; set; } = ServerCompatibilityMode.None;

        /// <summary>
        /// How long the database connection timeout should be
        /// </summary>
        public int ConnectionTimeout { get; set; } = 20;

        /// <summary>
        /// How long the command timeout should be
        /// </summary>
        public int CommandTimeout { get; set; } = 20;
        
        /// <summary>
        /// Whether to use connection pooling
        /// </summary>
        public bool Pooling { get; set; }

        /// <summary>
        /// The minimum connection pool size
        /// </summary>
        public int MinPoolSize { get; set; }

        /// <summary>
        /// The maximum connection pool size
        /// </summary>
        public int MaxPoolSize { get; set; } = 20;

        /// <summary>
        /// The interval time between connection pruning
        /// </summary>
        public int ConnectionPruningInterval { get; set; } = 5;

        /// <summary>
        /// The Postgres database schema to use
        /// </summary>
        public string Schema { get; set; } = "public";

        /// <summary>
        /// Maps this instance of <see cref="PostgresEventSourcingOptions" /> to an instance of <see cref="PostgresOptions" />
        /// </summary>
        /// <returns>A new instance of <see cref="PostgresOptions" /> using this instance of <see cref="PostgresEventSourcingOptions" /> as seed data</returns>
        public PostgresOptions ToPostgresOptions()
            => TinyMapper.Map<PostgresOptions>(this);
    }
}