using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FoxProDbExtentionConnection
{
    /// <summary>
    /// Extension methods for setting up cross-origin resource sharing services in an <see cref="IServiceCollection" />.
    /// </summary>
    public static class FoxDbServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Add-FoxDb resource sharing services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddFoxDb([NotNull]this IServiceCollection services, [NotNull] Action<FoxDbOptions> connectionString)
        {
            ArgumentNullException.ThrowIfNull(services);

            ArgumentNullException.ThrowIfNull(connectionString);

            services.Configure(connectionString);
            services.TryAdd(ServiceDescriptor.Scoped<IFoxDbContext, FoxDbContext>());
            return services;
        }

        public static IServiceCollection AddFoxDb([NotNull] this IServiceCollection services, [NotNull] Action<IServiceProvider, FoxDbOptions> configuration)
        {
            ArgumentNullException.ThrowIfNull(services);

            ArgumentNullException.ThrowIfNull(configuration);

            //services.Configure(configuration.);
            services.TryAdd(ServiceDescriptor.Scoped<IFoxDbContext, FoxDbContext>());

            return services;
        }

    }
}
