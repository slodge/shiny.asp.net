using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesExample.Pages
{
    public class DateChartModel : PageModel
    {
        private readonly ILogger<DateChartModel> _logger;

        public DateChartModel(ILogger<DateChartModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
    }
}