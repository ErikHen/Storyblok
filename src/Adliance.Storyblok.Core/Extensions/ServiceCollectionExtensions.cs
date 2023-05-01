using System.Net.Http;
using Adliance.Storyblok.Core.Clients;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Adliance.Storyblok.Core.Extensions
{
    public static class ServiceExtension
    {
        public static void AddStoryblok(this IServiceCollection services)
        {
            services.AddHttpClient<StoryblokBaseClient>();
            // services.AddHttpContextAccessor();
            services.AddSingleton(s => new StoryblokStoryClient(s.GetService<IHttpClientFactory>()!, s.GetService<ILogger<StoryblokBaseClient>>()));
            //services.AddSingleton<StoryblokStoriesClient>();
            //services.AddSingleton<StoryblokDatasourceClient>();
        }

        //public static IServiceCollection AddStoryblok(this IServiceCollection services, StoryblokOptions options)
        //{


        //    services.Configure<StoryblokOptions>(storyblokOptions =>
        //    {
        //        if (storyblokOptions != null)
        //        {
        //            configurationSection?.Bind(storyblokOptions);
        //        }

        //        if (configure != null && storyblokOptions != null)
        //        {
        //            configure.Invoke(storyblokOptions);
        //        }
        //    });

        //    return AddStoryblok(services);
        //}

        //public static IServiceCollection AddStoryblok(this IServiceCollection services, Action<StoryblokOptions>? configure)
        //{
        //    return AddStoryblok(services, null, configure);
        //}
    }
}