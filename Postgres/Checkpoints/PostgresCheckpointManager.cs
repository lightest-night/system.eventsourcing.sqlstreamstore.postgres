using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using LightestNight.System.Utilities.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace LightestNight.System.EventSourcing.SqlStreamStore.Postgres.Checkpoints
{
    public class PostgresCheckpointManager
    {
        private readonly Func<NpgsqlConnection> _createConnection;
        private readonly PostgresEventSourcingOptions _options;
        private readonly Scripts.Scripts _scripts;
        private readonly ILogger<PostgresCheckpointManager> _logger;

        public PostgresCheckpointManager(IOptions<PostgresEventSourcingOptions> options,
            ILogger<PostgresCheckpointManager> logger)
        {
            _options = options.ThrowIfNull(nameof(options)).Value;
            _logger = logger.ThrowIfNull(nameof(logger));
            _scripts = new Scripts.Scripts(_options.Schema);

            _createConnection = () => new PostgresConnection(options).Build();
        }

        [SuppressMessage("ReSharper", "CA2100")]
        public async Task<long?> GetGlobalCheckpoint(CancellationToken cancellationToken = default)
        {
            _logger.LogTrace(new EventId(3, "Get Checkpoint"),
                $"Getting Checkpoint with Id '{Constants.GlobalCheckpointId}'");

            await using var connection = _createConnection();
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            await using var command = new NpgsqlCommand(_scripts.GetCheckpoint, connection);
            command.Parameters.Add(new NpgsqlParameter("@CheckpointId", DbType.String, 500)
            {
                Value = Constants.GlobalCheckpointId
            });

            return await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false) as long?;
        }

        [SuppressMessage("ReSharper", "CA2100")]
        public async Task SetGlobalCheckpoint(long? checkpoint, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace(new EventId(2, "Set Checkpoint"), $"Setting Global Checkpoint: '{checkpoint}'");
            await using var connection = _createConnection();
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            await using var command = new NpgsqlCommand(_scripts.SetCheckpoint, connection);
            command.Parameters.Add(new NpgsqlParameter("@CheckpointId", DbType.String, 500)
            {
                Value = Constants.GlobalCheckpointId
            });
            command.Parameters.AddWithValue("@Checkpoint", checkpoint);

            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }
        
        /// <summary>
        /// Creates a schema that will hold checkpoints, if the schema does not exist.
        /// Calls to this should be part of an application's deployment/upgrade process and
        /// not every time your application boots up.
        /// </summary>
        /// <param name="cancellationToken">Any <see cref="CancellationToken" /> used to marshall the operation</param>
        /// <returns></returns>
        [SuppressMessage("ReSharper", "CA2100")]
        public async Task CreateSchemaIfNotExists(CancellationToken cancellationToken = default)
        {
            _logger.LogTrace(new EventId(1, "Create Schema"), "Creating Checkpoint Schema");
            await using var connection = _createConnection();
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            await using var transaction =
                await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
            await using (var command = new NpgsqlCommand($"CREATE SCHEMA IF NOT EXISTS {_options.Schema}",
                transaction.Connection, transaction))
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);

            await using (var command = new NpgsqlCommand(_scripts.CreateSchema, transaction.Connection, transaction))
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);

            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}