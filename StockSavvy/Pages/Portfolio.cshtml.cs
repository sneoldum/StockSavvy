using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StockSavvy.Pages
{
    public class PortfolioModel : PageModel
    {
        private readonly ILogger<PortfolioModel> _logger;

        public PortfolioModel(ILogger<PortfolioModel> logger)
        {
            _logger = logger;
        }


        public void OnGet()
        {

            



        }
    }
}
