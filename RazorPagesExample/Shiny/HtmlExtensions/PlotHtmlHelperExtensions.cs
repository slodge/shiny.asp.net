using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;


namespace RazorPagesExample.Shiny.HtmlExtensions
{
    public static class PlotHtmlHelperExtensions
    {
        public static IHtmlContent ShinyPlotOutput(
            this IHtmlHelper helper,
           string outputId,
           string width = "100%",
           string height = "400px")
        {
            var builder = new TagBuilder("div");
            builder.GenerateId(outputId, "_");
            builder.AddCssClass("shiny-plot-output");
            builder.MergeAttribute("style", $"width:{width};height:{height};");
            return builder;
        }
    }
}
