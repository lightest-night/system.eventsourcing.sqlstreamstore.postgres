using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
    
            services.Configure(optionsAccessor);
            services.TryAddSingleton<PostgresConnection>();
            
            var serviceProvider = services.BuildServiceProvider();
            if (!(serviceProvider.GetService<IStreamStore>() is PostgresStreamStore))
            {
                services.AddSingleton<IStreamStore>(sp =>
                {
                    var connection = sp.GetRequiredService<PostgresConnection>().Build();
                    var streamStore = new PostgresStreamStore(new PostgresStreamStoreSettings(connection.ConnectionString)
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