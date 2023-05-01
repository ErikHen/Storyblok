using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using Adliance.Storyblok.Core.Converters;
using Microsoft.Extensions.Logging;

namespace Adliance.Storyblok.Core.Clients
{
    public abstract class StoryblokBaseClient
    {
       //protected readonly IMemoryCache MemoryCache;
        protected readonly ILogger<StoryblokBaseClient> Logger;
        protected readonly HttpClient Client;
       // internal static bool IsInEditor;
        //protected readonly StoryblokOptions Settings;

        protected StoryblokBaseClient(IHttpClientFactory clientFactory, ILogger<StoryblokBaseClient> logger) //, StoryblokOptions settings)
        {
            Client = clientFactory.CreateClient();
            Logger = logger;
          //  Settings = settings;

            ValidateSettings();
        }

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private void ValidateSettings()
        {
            if (string.IsNullOrWhiteSpace(StoryblokOptions.BaseUrl))
            {
                throw new Exception("Storyblok API URL is missing in app settings.");
            }

            if (string.IsNullOrWhiteSpace(StoryblokOptions.ApiKeyPreview))
            {
                throw new Exception("Storyblok preview API key is missing in app settings.");
            }

            if (string.IsNullOrWhiteSpace(StoryblokOptions.ApiKeyPublic))
            {
                throw new Exception("Storyblok public API key is missing in app settings.");
            }

            if (StoryblokOptions.CacheDurationSeconds < 0)
            {
                throw new Exception("Cache duration (in seconds) must be equal or greater than zero.");
            }

            if (!StoryblokOptions.SupportedCultures.Any())
            {
                throw new Exception("Define at least one supported culture.");
            }
        }

        //TODO: api key should be fetched per request, and then find out if to use preview or public key
       // protected string ApiKey => StoryblokOptions.ApiKeyPreview ?? ""; // Settings.IncludeDraftStories || IsInEditor ? (Settings.ApiKeyPreview ?? "") : (Settings.ApiKeyPublic ?? "");

        protected bool IsDefaultCulture(CultureInfo culture)
        {
            return IsDefaultCulture(culture.ToString());
        }

        private bool IsDefaultCulture(string culture)
        {
            return StoryblokOptions.SupportedCultures[0].Equals(culture, StringComparison.OrdinalIgnoreCase);
        }

        protected JsonSerializerOptions JsonOptions
        {
            get
            {
                var options = new JsonSerializerOptions();
                options.Converters.Add(new StoryblokComponentConverter());
                options.Converters.Add(new StoryblokDateConverter());
                options.Converters.Add(new StoryblokNullableDateConverter());
                options.Converters.Add(new StoryblokIntConverter());
                options.Converters.Add(new StoryblokNullableIntConverter());
                options.Converters.Add(new StoryblokStringConverter());
                options.Converters.Add(new StoryblokNullableStringConverter());
              //  options.Converters.Add(new StoryblokMarkdownConverter());
                options.Converters.Add(new StoryblokAssetConverter());
                return options;
            }
        }
    }
}