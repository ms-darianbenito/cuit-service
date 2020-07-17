using CuitService.TaxInfoProvider;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TaxInfoProviderServiceCollectionExtensions
    {
        public static IServiceCollection AddTaxInfoProvider(this IServiceCollection services)
        {
            services.ConfigureOptions<ConfigureTaxInfoProviderOptions>();

            services.AddTransient<ITaxInfoProviderService, TaxInfoProviderService>();

            return services;
        }
    }
}

