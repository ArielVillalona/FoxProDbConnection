using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
        public static IServiceCollection AddFoxDb(this IServiceCollection services,string dataFolderString)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.TryAdd(ServiceDescriptor.Scoped(_ => new FoxDbContext(dataFolderString)));

            return services;
        }
    }
}
