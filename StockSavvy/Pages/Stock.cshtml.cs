using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using StockSavvy.Services;
using static StockSavvy.Models.StockModel;

namespace StockSavvy.Pages
{
    public class StockModel : PageModel
    {
        private readonly ILogger<StockModel> _logger;

        public StockModel(ILogger<StockModel> logger)
        {
            _logger = logger;
        }
        [BindProperty]
        public string StockCode { get; set; }

        [BindProperty]
        public string DataOpen { get; set; }

        [BindProperty]
        public string DataClose { get; set; }

        [BindProperty]
        public string DataHigh { get; set; }

        [BindProperty]
        public string DataLow { get; set; }

        [BindProperty]
        public string DataVolume { get; set; }

        [BindProperty]
        public string DataSymbol { get; set; }

        [BindProperty]
        public string DataLast { get; set; }

        [BindProperty]
        public List<Data> DataList { get; set; }
        [BindProperty]
        public List<string> Date { get; set; }

        [BindProperty]
        public string key { get; set; }

        [BindProperty]
        public string dataExJson { get; set; }

        public IActionResult OnPostLogoutRequest(UserService userService)
        {
            if (HttpContext.Request.Cookies.Count > 0)
            {
                var siteCookies = HttpContext.Request.Cookies.Where(c => c.Key.Contains(".AspNetCore.") || c.Key.Contains("Microsoft.Authentication") || c.Key.Contains("username"));
                foreach (var cookie in siteCookies)
                {
                    Response.Cookies.Delete(cookie.Key);
                }
            }

            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToPage("/Index");

        }


        public void OnPostStockRequest(StockService stockService)

        {
            var stockModel = stockService.GetStockModel(StockCode,key);

            if (stockModel.json == "{\n    \"Note\": \"Thank you for using Alpha Vantage! Our standard API call frequency is 5 calls per minute and 500 calls per day. Please visit https://www.alphavantage.co/premium/ if you would like to target a higher API call frequency.\"\n}")
            {
                dataExJson = "Thank you for using Alpha Vantage! Our standard API call frequency is 5 calls per minute and 500 calls per day. Please visit https://www.alphavantage.co/premium/ if you would like to target a higher API call frequency.";

            }
            else if ((stockModel.json == "{\n    \"Error Message\": \"the parameter apikey is invalid or missing. Please claim your free API key on (https://www.alphavantage.co/support/#api-key). It should take less than 20 seconds.\"\n}"))
            {
                dataExJson =
                    "The parameter apikey is invalid or missing. Please claim your free API key on (https://www.alphavantage.co/support/#api-key). It should take less than 20 seconds.";
            }
            else if (stockModel.json == "{\n    \"Error Message\": \"Invalid API call. Please retry or visit the documentation (https://www.alphavantage.co/documentation/) for TIME_SERIES_INTRADAY.\"\n}")
            {
                dataExJson =
                    "Invalid API call. Please retry or visit the documentation (https://www.alphavantage.co/documentation/) for TIME_SERIES_INTRADAY.";
            }else
            {
                dataExJson = null;
            }

            if (stockModel != null&& dataExJson==null)
            {
                var firstOrDefaultValue = stockModel.Datas.Values.FirstOrDefault();

                Date = stockModel.Datas.Keys.ToList();
                DataList = stockModel.Datas.Values.ToList();
                DataSymbol = stockModel.metaData.Symbol;
                DataLast = stockModel.metaData.LastRefreshed;
                DataOpen = firstOrDefaultValue.Open;
                DataClose = firstOrDefaultValue.Close;
                DataHigh = firstOrDefaultValue.High;
                DataLow = firstOrDefaultValue.Low;
                DataVolume = firstOrDefaultValue.Volume;
            }
        }
        public void OnGet()
        {

        }
    }
}