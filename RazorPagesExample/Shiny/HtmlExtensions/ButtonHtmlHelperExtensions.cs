using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesExample.Shiny.HtmlExtensions
{
    public static class ButtonHtmlHelperExtensions
    {
        public static IHtmlContent ShinyActionButtonInput(
            this IHtmlHelper helper,
            string inputId,
            string label)
        {
            var builder = new TagBuilder("button");
            builder.AddCssClass("action-button");
            builder.GenerateId(inputId, "_");
            builder.InnerHtml.SetContent(label);
            return builder;
        }
    }
}
