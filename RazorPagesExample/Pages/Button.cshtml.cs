using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesExample.Pages
{
    public class ButtonModel : PageModel
    {
        private readonly ILogger<ButtonModel> _logger;

        public ButtonModel(ILogger<ButtonModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
    }
}