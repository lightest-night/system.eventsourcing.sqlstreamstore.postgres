using System;
using System.Reflection;
using LightestNight.System.EventSourcing.Checkpoints;
using LightestNight.System.EventSourcing.SqlStreamStore.Postgres.Checkpoints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SqlStreamStore;

namespace LightestNight.System.EventSourcing.SqlStreamStore.Postgres
{
    public static class ExtendsServiceCollection
    {
        public static IServiceCollection AddPostgresEventStore(this IServiceCollection services,
            Action<PostgresEventSourcingOptions>? optionsAccessor = null, params Assembly[] eventAssemblies)
        {
            var postgresOptions = new PostgresEventSourcingOptions();
            optionsAccessor?.Invoke(postgresOptions);
            // ReSharper disable once RedundantAssignment
            services.AddEventStore(eventSourcingOptions => eventSourcingOptions = postgresOptions, eventAssemblies)
                .Configure(optionsAccessor)
                .AddSingleton<PostgresCheckpointManager>()
                .TryAddSingleton<PostgresConnection>();

            var serviceProvider = services.BuildServiceProvider();
            if (!(serviceProvider.GetService<IStreamStore>() is PostgresStreamStore))
            {
                services.AddSingleton<IStreamStore>(sp =>
                {
                    var connection = sp.GetRequiredService<PostgresConnection>().Build();
                    var streamStore = new PostgresStreamStore(
                        new PostgresStreamStoreSettings(connection.ConnectionString)
                        {
                            Schema = postgresOptions.Schema
                        });

                    if (!postgresOptions.CreateSchemaIfNotExists)
                        return streamStore;

                    streamStore.CreateSchemaIfNotExists().Wait();
                    sp.GetRequiredService<PostgresCheckpointManager>().CreateSchemaIfNotExists().Wait();

                    return streamStore;
                });
            }

            services.TryAddSingleton<GetGlobalCheckpoint>(sp =>
                sp.GetRequiredService<PostgresCheckpointManager>().GetGlobalCheckpoint);
            
            services.TryAddSingleton<SetGlobalCheckpoint>(sp =>
                sp.GetRequiredService<PostgresCheckpointManager>().SetGlobalCheckpoint);

            return services;
        }
    }
}