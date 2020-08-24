using System;
using Microsoft.Extensions.DependencyInjection;
using SqlStreamStore;

namespace LightestNight.System.EventSourcing.SqlStreamStore.Postgres
{
    public static class ExtendsServiceCollection
    {
        public static IServiceCollection AddPostgresEventStore(this IServiceCollection services,
            Action<PostgresEventSourcingOptions>? optionsAccessor = null)
        {
            var postgresOptions = new PostgresEventSourcingOptions();
            optionsAccessor?.Invoke(postgresOptions);

            // ReSharper disable once RedundantAssignment
            services.AddEventStore(eventSourcingOptionsAccessor: options => options = postgresOptions);

            var serviceProvider = services.BuildServiceProvider();
            if (!(serviceProvider.GetService<IStreamStore>() is PostgresStreamStore))
            {
                services.AddSingleton<IStreamStore>(sp =>
                {
                    var connection = new PostgresConnection(postgresOptions).Build();
                    var streamStore = new PostgresStreamStore(
                        new PostgresStreamStoreSettings(connection.ConnectionString)
                        {
                            Schema = postgresOptions.Schema
                        });

                    if (postgresOptions.CreateSchemaIfNotExists)
                        streamStore.CreateSchemaIfNotExists().Wait();

                    return streamStore;
                });
            }

            return services;
        }
    }
}