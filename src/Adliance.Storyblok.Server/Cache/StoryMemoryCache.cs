using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Adliance.Storyblok.Core.Cache;
using Adliance.Storyblok.Core.Components;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Adliance.Storyblok.Server.Cache
{
    public class StoryMemoryCache : IStoryCache
    {
        private IMemoryCache _memoryCache;
        public StoryMemoryCache(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void Set(string key, StoryblokStory story, TimeSpan expiresAfter)
        {
            _memoryCache.Set(key, story, expiresAfter);
        }

        public StoryblokStory? Get(string key)
        {
            if (_memoryCache.TryGetValue(key, out object? cachedStory) && cachedStory != null)
            {
                return (StoryblokStory)cachedStory;
            }
            return null;
        }

        public void Clear()
        {
            _memoryCache.Dispose();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
        }
    }
}
