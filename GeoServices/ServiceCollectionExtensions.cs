using Microsoft.Extensions.DependencyInjection;

namespace GeoServices
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWeatherService<TService>(this IServiceCollection services) where TService : class, IWeatherService
        {
            ArgumentNullException.ThrowIfNull(services, nameof(services));

            services.AddHttpClient("WeatherService").AddTypedClient<IWeatherService, TService>();
            return services;
        }

        public static IServiceCollection AddWeatherService<TService>(this IServiceCollection services, Action<WeatherServiceOptions> options) where TService : class, IWeatherService
        {
            ArgumentNullException.ThrowIfNull(services, nameof(services));
            ArgumentNullException.ThrowIfNull(options, nameof(options));

            services.Configure(options);
            services.AddHttpClient("WeatherService").AddTypedClient<IWeatherService, TService>();
            return services;
        }

        public static IServiceCollection AddGeoService<TService>(this IServiceCollection services) where TService : class, IGeoIPService
        {
            ArgumentNullException.ThrowIfNull(services, nameof(services));

            services.AddTransient<IGeoIPService, TService>();
            services.AddHttpClient<TService>();
            return services;
        }

        public static IServiceCollection AddGeoService<TService>(this IServiceCollection services, Action<GeoIPServiceOptions> options) where TService : class, IGeoIPService
        {
            ArgumentNullException.ThrowIfNull(services, nameof(services));
            ArgumentNullException.ThrowIfNull(options, nameof(options));

            var opt = new GeoIPServiceOptions();
            options(opt);

            services.Configure(options);
            services.AddTransient<IGeoIPService, TService>();

            if (opt.CacheDuration != null)
            {
                services.AddHttpClient<TService>();
            }
            else
            {
                services.AddHttpClient<TService>();
            }

            return services;
        }
    }
}
