using System;
using Adliance.Storyblok.Core.Components;

namespace Adliance.Storyblok.Core.Cache;

public interface IStoryCache
{
    void Set(string key, StoryblokStory story, TimeSpan expiresAfter);
    StoryblokStory? Get(string key);
    void Clear();   
}