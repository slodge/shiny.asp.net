using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;


namespace RazorPagesExample.Shiny.HtmlExtensions
{
    public static class SliderHtmlHelperExtensions
    {
        // NOTE - this is a very simple numeric slider
        //   - didn't copy all the options from sliderInput
        //   - full implementation not needed for a demo
        public static IHtmlContent ShinySliderInput(
            this IHtmlHelper helper,
           string inputId,
           string label,
           int min,
           int max,
           int value,
           int step = 1)
        {
            var builder = new TagBuilder("div");
            builder.AddCssClass("form-group");
            builder.AddCssClass($"shiny-input-container");

            var labelBuilder = new TagBuilder("label");
            labelBuilder.GenerateId($"{inputId}-label", "_");
            labelBuilder.AddCssClass("control-label");
            labelBuilder.MergeAttribute("for", inputId);
            labelBuilder.InnerHtml.SetContent(label);

            var inputBuilder = new TagBuilder("input");
            inputBuilder.GenerateId(inputId, "_");
            inputBuilder.AddCssClass("js-range-slider");
            inputBuilder.MergeAttribute("data-skin", "shiny");
            inputBuilder.MergeAttribute("data-min", min.ToString());
            inputBuilder.MergeAttribute("data-max", max.ToString());
            inputBuilder.MergeAttribute("data-from", value.ToString());
            inputBuilder.MergeAttribute("data-step", step.ToString());
            inputBuilder.MergeAttribute("data-keyboard", "true");

            builder.InnerHtml.AppendHtml(labelBuilder);
            builder.InnerHtml.AppendHtml(inputBuilder);

            return builder;
            /*
            // `data - type` = if (length(value) > 1) "double",
            //$"data-to={to} " +
            // `data - grid` = ticks,
            // `data - grid - num` = n_ticks,
            // `data - grid - snap` = FALSE,
            // `data - prettify - separator` = sep,
            // `data - prettify - enabled` = (sep != ""),
            // `data - prefix` = pre,
            // `data - postfix` = post,
            // # This value is only relevant for range sliders; for non-range sliders it
            // # causes problems since ion.RangeSlider 2.1.2 (issue #1605).
            // `data - drag - interval` = if (length(value) > 1) dragRange,
            // # The following are ignored by the ion.rangeSlider, but are used by Shiny.
            // `data - data - type` = dataType,
            // `data - time - format` = timeFormat,
            // `data - timezone` = timezone

            */
        }
    }
}
