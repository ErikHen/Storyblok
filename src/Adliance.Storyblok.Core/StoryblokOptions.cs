using System.Collections.Generic;
using Adliance.Storyblok.Core.Clients;

namespace Adliance.Storyblok.Core
{
    public static class StoryblokOptions
    {
        public static string? ApiKeyPreview { get; set; }
        public static string? ApiKeyPublic { get; set; }
        public static bool IncludeDraftStories { get; set; }
        public static string BaseUrl { get; set; } = "https://api.storyblok.com/v1/cdn";

        /// <summary>
        /// The duration (in seconds) that all loaded stories will be cached locally.
        /// </summary>
        public static int CacheDurationSeconds { get; set; } = 60 * 15;

        /// <summary>
        /// If this value is not empty, than a call to the root ~/ will be handled with the specified slug.
        /// </summary>
        public static string HandleRootWithSlug { get; set; } = "/home";

        /// <summary>
        /// All story slugs defined here will not be automatically mapped.
        /// </summary>
        public static IList<string> IgnoreSlugs { get; set; } = new List<string>();

        /// <summary>
        /// The supported cultures, as specified in Storyblok.
        /// The first culture is also the default culture.
        /// </summary>
        public static string[] SupportedCultures { get; set; } = new string[0];

        /// <summary>
        /// This is the slug that will be loaded from Storyblok as part of the health check middleware.
        /// </summary>
        //  public string SlugForHealthCheck { get; set; } = "home";

        /// <summary>
        /// If set, the middleware will clear all caches when this slug is being called.
        /// This is useful for using it as the Storyblok webhook callback on content changes.
        /// </summary>
        // public string SlugForClearingCache { get; set; } = "/clear-storyblok-cache";

        //  public string CultureCookieName { get; set; } = "culture";
        //  public bool EnableSitemap { get; set; } = true;

        public static bool ResolveAssets { get; set; } = false;
        public static ResolveLinksType ResolveLinks { get; set; } = ResolveLinksType.Url;
        public static string ResolveRelations { get; set; } = "";
        
        /// <summary>
        /// The name of the datasource that contains HTTP redirect information. Leave empty to not use the RedirectsMiddleware at all.
        /// </summary>
        //public string? RedirectsDatasourceName { get; set; }
        
       // public Func<StoryblokStory, bool> SitemapFilter { get; set; } = _ => true;
    }

    
}