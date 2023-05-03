using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Adliance.Storyblok.Core.Cache;
using Adliance.Storyblok.Core.Components;
using Adliance.Storyblok.Core.Extensions;
using Microsoft.Extensions.Logging;

namespace Adliance.Storyblok.Core.Clients
{
    public class StoryblokStoryClient : StoryblokBaseClient
    {
        private IStoryCache _storyCache;

        public StoryblokStoryClient(IHttpClientFactory clientFactory, ILogger<StoryblokBaseClient> logger, IStoryCache storyCache) : base(clientFactory, logger)
        {
            _storyCache = storyCache;
        }

        public StoryblokStoryQuery Story()
        {
            return new(this);
        }

        internal async Task<StoryblokStory<T>?> LoadStory<T>(CultureInfo? culture, string slug, ResolveLinksType resolveLinks, bool resolveAssets, string resolveRelations) where T : StoryblokComponent
        {
            var story = await LoadStory(culture, slug, resolveLinks, resolveAssets, resolveRelations);
            if (story == null)
            {
                return null; 
            }

            return new StoryblokStory<T>(story);
        }

        internal async Task<StoryblokStory?> LoadStory(CultureInfo? culture, string slug, ResolveLinksType resolveLinks, bool resolveAssets, string resolveRelations)
        {
            var IsInEditor = slug.IsStoryblokEditorUrl();

            if (IsInEditor)
            {
                return await LoadStoryFromStoryblok(culture, slug, resolveLinks, resolveAssets, resolveRelations);
            }

            var cacheKey = $"{culture}_{slug}_{resolveLinks}_{resolveAssets}_{resolveRelations}";
            var cachedStory = _storyCache.Get(cacheKey);
            if (cachedStory != null)
            {
                Logger.LogTrace($"Using cached story for slug \"{slug}\" (culture \"{culture}\").");
                return cachedStory;
            }

            var cacheKeyUnavailable = "404_" + cacheKey;
            cachedStory = _storyCache.Get(cacheKeyUnavailable);
            if (cachedStory != null)
            {
                Logger.LogTrace($"Using cached 404 for slug \"{slug}\" (culture \"{culture}\").");
                return null;
            }

            Logger.LogTrace($"Trying to load story for slug \"{slug}\" (culture \"{culture}\").");
            var story = await LoadStoryFromStoryblok(culture, slug, resolveLinks, resolveAssets, resolveRelations);
            if (story != null)
            {
                Logger.LogTrace($"Story loaded for slug \"{slug}\" (culture \"{culture}\").");
                _storyCache.Set(cacheKey, story, TimeSpan.FromSeconds(StoryblokOptions.CacheDurationSeconds));
                return story;
            }

            Logger.LogTrace($"Story not found for slug \"{slug}\" (culture \"{culture}\").");
            _storyCache.Set(cacheKeyUnavailable, new StoryblokStory(), TimeSpan.FromSeconds(StoryblokOptions.CacheDurationSeconds));
            return null;
        }

        private async Task<StoryblokStory?> LoadStoryFromStoryblok(CultureInfo? culture, string slug, ResolveLinksType resolveLinks, bool resolveAssets, string resolveRelations)
        {
            //remove querystring from url
            var slugPath = slug.Split('?')[0];
            var apiKey = slug.IsStoryblokEditorUrl() ? StoryblokOptions.ApiKeyPreview : StoryblokOptions.ApiKeyPublic;

            //TODO: culture shoudl always be in slug(?)
            var defaultCulture = new CultureInfo(StoryblokOptions.SupportedCultures.First());
            var currentCulture = defaultCulture;

            if (culture != null)
            {
                // only use the culture if it's actually supported
                // use only the short culture "en", if we get a full culture "en-US" but only support the short one
                var matchingCultures = StoryblokOptions.SupportedCultures
                    .Where(x => x.Equals(culture.ToString(), StringComparison.OrdinalIgnoreCase) || x.Equals(culture.TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(x => x.Length)
                    .ToArray();

                if (matchingCultures.Any())
                {
                    currentCulture = new CultureInfo(matchingCultures.First());
                }
            }

            var url = $"{StoryblokOptions.BaseUrl}/stories/{slugPath}?token={apiKey}";
            if (!currentCulture.Equals(defaultCulture))
            {
                url = $"{StoryblokOptions.BaseUrl}/stories/{currentCulture.ToString().ToLower()}/{slugPath}?token={apiKey}";
            }

            url += $"&cb={DateTime.UtcNow:yyyyMMddHHmmss}";

            if (resolveLinks != ResolveLinksType.None)
            {
                url += $"&resolve_links={resolveLinks.ToString().ToLower()}";
            }

            // should only work in Premium Plans, (as per https://www.storyblok.com/docs/api/content-delivery/v2)
            if (resolveAssets)
            {
                url += "&resolve_assets=1";
            }

            if (!string.IsNullOrWhiteSpace(resolveRelations))
            {
                url += $"&resolve_relations={resolveRelations}";
            }

            if (StoryblokOptions.IncludeDraftStories || slug.IsStoryblokEditorUrl())
            {
                url += "&version=draft";
            }

            Logger.LogTrace($"Loading {url} ...");

            var response = await Client.GetAsync(url);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            var story = JsonSerializer.Deserialize<StoryblokStoryContainer>(responseString, JsonOptions)?.Story;
            if (story == null)
            {
                throw new Exception($"Unable to deserialize {responseString}.");
            }

            story.LoadedAt = DateTime.UtcNow;
            return story;
        }
    }
}