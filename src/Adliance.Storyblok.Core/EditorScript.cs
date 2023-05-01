using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adliance.Storyblok.Core.Extensions;

namespace Adliance.Storyblok.Core
{
    public static class Script
    {
        public static string StoryblokBridgeScript(string url)
        {
            var scriptElements = string.Empty;
            if (url.IsStoryblokEditorUrl())
            {
                scriptElements = $"<script src=\"//app.storyblok.com/f/storyblok-v2-latest.js\"></script>"
                    + "<script>const storyblokInstance = new StoryblokBridge(); storyblokInstance.on([\"change\", \"published\"], () => { window.location.reload(); });</script>";
            }
            
            return scriptElements;
        }
    }
}
