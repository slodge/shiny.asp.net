using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;


namespace RazorPagesExample.Shiny.HtmlExtensions
{
    public static class VerbatimTextHtmlHelperExtensions
    {
        public static IHtmlContent VerbatimTextOutput(
            this IHtmlHelper helper,
           string outputId)
        {
            var builder = new TagBuilder("pre");
            builder.AddCssClass("shiny-text-output");
            builder.AddCssClass("noplaceholder");
            builder.GenerateId(outputId, "_");
            return builder;
        }
    }
}
