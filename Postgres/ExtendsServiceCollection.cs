using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddEventStore(eventSourcingOptions => eventSourcingOptions = postgresOptions, eventAssemblies);

            services.Configure(optionsAccessor);
            var serviceProvider = services.BuildServiceProvider();
            if (!(serviceProvider.GetService<IStreamStore>() is PostgresStreamStore))
            {
                services.AddSingleton<IStreamStore>(sp =>
                {
                    var connection = sp.GetRequiredService<PostgresConnection>().Build();
                    var streamStore = new PostgresStreamStore(new PostgresStreamStoreSettings(connection.ConnectionString));
                    
                    if (postgresOptions.CreateSchemaIfNotExists)
                        streamStore.CreateSchemaIfNotExists().Wait();

                    return streamStore;
                });
            }

            return services;
        }
    }
}