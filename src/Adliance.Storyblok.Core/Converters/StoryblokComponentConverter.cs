﻿using System;
using System.Text.Json;
using Adliance.Storyblok.Core.Components;

namespace Adliance.Storyblok.Core.Converters
{
    public class StoryblokComponentConverter : System.Text.Json.Serialization.JsonConverter<StoryblokComponent>
    {
        public override StoryblokComponent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // performance is probably abysmal, but System.Text.Json does not support polymorphic deserialization very well
            // https://github.com/dotnet/corefx/issues/38650
            using var doc = JsonDocument.ParseValue(ref reader);
            if (doc.RootElement.TryGetProperty("component", out var componentElement))
            {
                var componentName = componentElement.GetString();
                if (!string.IsNullOrWhiteSpace(componentName))
                {
                    var componentMappings = StoryblokMappings.Mappings;

                    if (componentMappings.ContainsKey(componentName))
                    {
                        var mapping = componentMappings[componentName];

                        var rawText = doc.RootElement.GetRawText();
                        try
                        {
                            if (!(JsonSerializer.Deserialize(rawText, mapping.Type, options) is StoryblokComponent component))
                            {
                                throw new Exception($"Unable to serialize {rawText}");
                            }

                            // we don't want the "editable" property set at all, when we're not in editor
                            // this makes it easier for the client, so he does not have to check if in the editor on each component, he just has to render the "editable" stuff into it
                            // TODO if (!StoryblokBaseClient.IsInEditor)
                            //{
                            //    component.Editable = null;
                            //}

                            //TODO: component.IsInEditor = StoryblokBaseClient.IsInEditor;

                            return component;
                        }
                        catch (JsonException ex)
                        {
                            throw new Exception($"Unable to deserialize ({ex.Message}): {rawText}", ex);
                        }
                    }
                }
            }

            // don't call JsonSerializer.Deserizalize, because we'll get a stack overlow
            return new StoryblokComponent
            {
                Uuid = doc.RootElement.GetProperty("_uid").GetGuid(),
                Component = doc.RootElement.GetProperty("component").GetString() ?? "",
                //TODO: IsInEditor = StoryblokBaseClient.IsInEditor
            };
        }

        public override void Write(Utf8JsonWriter writer, StoryblokComponent value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}