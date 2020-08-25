using AutoFixture.Xunit2;
using ExpectedObjects;
using LightestNight.System.Data.Postgres;
using Xunit;

namespace LightestNight.System.EventSourcing.SqlStreamStore.Postgres.Tests
{
    public class PostgresEventSourcingOptionsTests
    {
        [Theory, AutoData]
        public void ShouldMapToPostgresOptionsSuccessfully(PostgresEventSourcingOptions sut)
        {
            // Arrange
            var expected = new PostgresOptions
            {
                Host = sut.Host,
                Port = sut.Port,
                Database = sut.Database,
                Username = sut.Username,
                Password = sut.Password,
                SslMode = sut.SslMode,
                TrustServerCertificate = sut.TrustServerCertificate,
                ServerCompatibilityMode = sut.ServerCompatibilityMode,
                ConnectionTimeout = sut.ConnectionTimeout,
                CommandTimeout = sut.CommandTimeout,
                Pooling = sut.Pooling,
                MinPoolSize = sut.MinPoolSize,
                MaxPoolSize = sut.MaxPoolSize,
                ConnectionPruningInterval = sut.ConnectionPruningInterval,
                Schema = sut.Schema
            }.ToExpectedObject();
            
            // Act
            var result = sut.ToPostgresOptions();
            
            // Assert
            expected.ShouldEqual(result);
        }
    }
}