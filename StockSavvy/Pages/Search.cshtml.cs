using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StockSavvy.Pages
{

    public class SearchModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Query { get; set; }

        public List<string> Results { get; set; } = new List<string>();

        public void OnGet()
        {
            if (!string.IsNullOrEmpty(Query))
            {
                // TODO: Get stock data from API
                // TODO: Draw chart
                // TODO: Add button named "Buy" and "Sell"
            }
        }
    }
}