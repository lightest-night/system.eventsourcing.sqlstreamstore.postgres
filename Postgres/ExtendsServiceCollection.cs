using System;
using LightestNight.System.Data.Postgres;
using Microsoft.Extensions.DependencyInjection;
using SqlStreamStore;

// ReSharper disable once RedundantAssignment

namespace LightestNight.System.EventSourcing.SqlStreamStore.Postgres
{
    public static class ExtendsServiceCollection
    {
        public static IServiceCollection AddPostgresEventStore(this IServiceCollection services,
            Action<PostgresEventSourcingOptions>? options = null)
        {
            var postgresEventSourcingOptions = new PostgresEventSourcingOptions();
            options?.Invoke(postgresEventSourcingOptions);

            services.AddPostgresData(postgresOptions =>
                    postgresOptions = postgresEventSourcingOptions.ToPostgresOptions())
                .AddEventStore(eventSourcingOptionsAccessor: eventSourcingOptions =>
                    eventSourcingOptions = postgresEventSourcingOptions);

            var serviceProvider = services.BuildServiceProvider();
            if (!(serviceProvider.GetService<IStreamStore>() is PostgresStreamStore))
            {
                services.AddSingleton<IStreamStore>(sp =>
                {
                    var connectionString = sp.GetRequiredService<IPostgresConnection>().Build().ConnectionString;
                    var streamStore = new PostgresStreamStore(new PostgresStreamStoreSettings(connectionString));

                    if (postgresEventSourcingOptions.CreateSchemaIfNotExists)
                        streamStore.CreateSchemaIfNotExists().Wait();

                    return streamStore;
                });
            }

            return services;
        }
    }
}