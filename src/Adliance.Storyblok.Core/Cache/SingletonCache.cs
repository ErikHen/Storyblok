using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adliance.Storyblok.Core.Components;

namespace Adliance.Storyblok.Core.Cache
{
    public class SingletonCache : IStoryCache
    {
        private readonly Dictionary<string, CacheContainer> _cache = new();

        public void Set(string key, StoryblokStory story, TimeSpan expiresAfter)
        {
            _cache[key] = new CacheContainer(story, expiresAfter);
        }

        public StoryblokStory? Get(string key)
        {
            if (_cache.TryGetValue(key, out var cacheEntry))
            {
                if (cacheEntry.ExpiresDateTimeUtc < DateTime.UtcNow)
                {
                    _cache.Remove(key);
                    return null;
                }
                return cacheEntry.Story;
            }

            return null;
        }

        public void Clear()
        {
            _cache.Clear();
        }
    }

    internal class CacheContainer
    {
        public DateTime ExpiresDateTimeUtc { get; set; }
        public StoryblokStory Story { get; set; }

        public CacheContainer(StoryblokStory story, TimeSpan expiresAfter)
        {
            Story = story;
            ExpiresDateTimeUtc = DateTime.UtcNow.Add(expiresAfter);
        }
    }
}
