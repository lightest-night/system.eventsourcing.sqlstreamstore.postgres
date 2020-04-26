using AutoFixture;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;

namespace LightestNight.System.EventSourcing.SqlStreamStore.Postgres.Tests
{
    public class PostgresConnectionTests
    {
        private readonly PostgresEventSourcingOptions _options;
        private readonly PostgresConnection _sut;

        public PostgresConnectionTests()
        {
            var fixture = new Fixture();
            _options = fixture
                .Build<PostgresEventSourcingOptions>()
                .Without(o => o.MinPoolSize)
                .Without(o => o.MaxPoolSize)
                .Do(o =>
                {
                    o.MinPoolSize = fixture.Create<int>();
                    o.MaxPoolSize = o.MinPoolSize + fixture.Create<int>();
                })
                .Create();
            
            _sut = new PostgresConnection(Options.Create(_options));
        }

        [Fact]
        public void Should_Create_New_Postgres_Connection_And_Populate_ConnectionString()
        {
            // Act
            var connectionString = _sut.Build().ConnectionString;
            
            // Assert
            connectionString.ShouldContain($"Host={_options.Host}");
            connectionString.ShouldContain($"Port={_options.Port}");
            connectionString.ShouldContain($"Database={_options.Database}");
            connectionString.ShouldContain($"Username={_options.Username}");
            connectionString.ShouldContain($"Password={_options.Password}");
            connectionString.ShouldContain($"SSL Mode={_options.SslMode}");
            connectionString.ShouldContain($"Trust Server Certificate={_options.TrustServerCertificate}");
            connectionString.ShouldContain($"Server Compatibility Mode={_options.ServerCompatibilityMode}");
            connectionString.ShouldContain($"Timeout={_options.ConnectionTimeout}");
            connectionString.ShouldContain($"Command Timeout={_options.CommandTimeout}");
            connectionString.ShouldContain($"Pooling={_options.Pooling}");
            connectionString.ShouldContain($"Minimum Pool Size={_options.MinPoolSize}");
            connectionString.ShouldContain($"Maximum Pool Size={_options.MaxPoolSize}");
            connectionString.ShouldContain($"Connection Pruning Interval={_options.ConnectionPruningInterval}");
        }
    }
}