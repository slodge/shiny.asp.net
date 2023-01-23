using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using ScottPlot.Palettes;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Emit;

namespace RazorPagesExample.Shiny.HtmlExtensions
{
    public static class DateRangeHtmlHelperExtensions
    {
        public static IHtmlContent ShinyDateRangeInput(
            this IHtmlHelper helper,
            string inputId,
            string label, 
            DateTime? start = null,
            DateTime? end = null,
            DateTime? min = null,
            DateTime? max = null,
            string format = "yyyy-mm-dd", // TODO - check this... MM or mm in this js control?
            string startview = "month",
            int weekstart = 0, 
            string language = "en",
            string separator = " to ", 
            bool autoclose = true)
        {
            var builder = new TagBuilder("div");
            builder.AddCssClass("shiny-date-range-input");
            builder.AddCssClass("form-group");
            builder.AddCssClass("shiny-input-container");
            builder.GenerateId(inputId, "_");

            var labelBuilder = new TagBuilder("label");
            labelBuilder.GenerateId($"{inputId}-label", "_");
            labelBuilder.AddCssClass("control-label");
            labelBuilder.MergeAttribute("for", inputId);
            labelBuilder.InnerHtml.SetContent(label);
            builder.InnerHtml.AppendHtml(labelBuilder);

            var rangeBuilder = new TagBuilder("div");
            rangeBuilder.AddCssClass("input-daterange");
            rangeBuilder.AddCssClass("input-group");
            rangeBuilder.AddCssClass("input-group-sm");

            var input1Builder = new TagBuilder("input");
            input1Builder.AddCssClass("form-control");
            input1Builder.MergeAttribute("type", "text");
            input1Builder.MergeAttribute("aria-labelledby", $"{inputId}-label");
            input1Builder.MergeAttribute("title", $"Date format:{format}");
            input1Builder.MergeAttribute("data-date-language", language);
            input1Builder.MergeAttribute("data-date-week-start", weekstart.ToString());
            input1Builder.MergeAttribute("data-date-format", format);
            input1Builder.MergeAttribute("data-date-start-view", startview);
            input1Builder.MergeAttribute("data-min-date", min?.ToString("yyyy-MM-dd"));
            input1Builder.MergeAttribute("data-max-date", max?.ToString("yyyy-MM-dd"));
            input1Builder.MergeAttribute("data-initial-date", start?.ToString("yyyy-MM-dd"));
            input1Builder.MergeAttribute("data-date-autoclose", autoclose.ToString());

            var spanBuilder = new TagBuilder("span");
            spanBuilder.AddCssClass("input-group-addon");
            spanBuilder.AddCssClass("input-group-prepend");
            spanBuilder.AddCssClass("input-group-append");
            spanBuilder.InnerHtml.AppendHtml($"<span class='input-group-text'>{separator}</span>");

            var input2Builder = new TagBuilder("input");
            input2Builder.AddCssClass("form-control");
            input2Builder.MergeAttribute("type", "text");
            input2Builder.MergeAttribute("aria-labelledby", $"{inputId}-label");
            input2Builder.MergeAttribute("title", $"Date format:{format}");
            input2Builder.MergeAttribute("data-date-language", language);
            input2Builder.MergeAttribute("data-date-week-start", weekstart.ToString());
            input2Builder.MergeAttribute("data-date-format", format);
            input2Builder.MergeAttribute("data-date-start-view", startview);
            input2Builder.MergeAttribute("data-min-date", min?.ToString("yyyy-MM-dd"));
            input2Builder.MergeAttribute("data-max-date", max?.ToString("yyyy-MM-dd"));
            input2Builder.MergeAttribute("data-initial-date", end?.ToString("yyyy-MM-dd"));
            input2Builder.MergeAttribute("data-date-autoclose", autoclose.ToString());

            rangeBuilder.InnerHtml.AppendHtml(input1Builder);
            rangeBuilder.InnerHtml.AppendHtml(spanBuilder);
            rangeBuilder.InnerHtml.AppendHtml(input2Builder);

            builder.InnerHtml.AppendHtml(rangeBuilder);

            return builder;
        }
    }
}
