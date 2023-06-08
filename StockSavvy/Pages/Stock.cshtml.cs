using System.Globalization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson;
using Newtonsoft.Json;
using StockSavvy.Models;
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

        [BindProperty]
        public int Amount { get; set; }

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
            var stockModel = stockService.GetStockModel(StockCode, key);

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
            }
            else
            {
                dataExJson = null;
            }

            if (stockModel != null && dataExJson == null)
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

        public void OnPostCryptoRequest(StockService stockService)
        {
            var stockModel = stockService.GetCryptoModel(StockCode);
            
            DataClose = stockModel.price;

        }
        public IActionResult OnPostBuyRequest(PortfolioService portfolioService, StockService stockService)
        {
            var userName = Request.Cookies["username"];
            var dataClose = Convert.ToDouble(DataClose.Replace(".", ","));
            var UserService = new UserService();
            var user = UserService.GetOneByUsername(userName);

            var userPortfolio = portfolioService.GetOneByUserId(user.Id);
            if (userPortfolio == null)
            {
                var portfolioModel = new PortfolioMongoModel
                {
                    UserId = user.Id,
                    Stocks = new List<string>(),
                    Status = true
                };
                portfolioService.Create(portfolioModel);
                userPortfolio = portfolioService.GetOneByUserId(user.Id);

            }

            foreach (var stockId in userPortfolio.Stocks)
            {
                var stock = stockService.GetOneById(new ObjectId(stockId));
               

                if (stock.StockCode == StockCode)
                {
                    var newTotalCost = (stock.AverageCost * stock.Amount) + (Convert.ToDouble(dataClose) * Amount);
                    var newTotalAmount = stock.Amount + Amount;
                    stock.AverageCost = newTotalCost / newTotalAmount;
                    stock.Amount = newTotalAmount;
                    stockService.Update(stock);
                    return RedirectToPage("/Portfolio");
                }
            }

            var stockModel = new StockMongoModel
            {
                Amount = Amount,
                AverageCost = (Convert.ToDouble(dataClose)),
                StockCode = StockCode,
                Status = true
            };

            stockService.Create(stockModel);
            userPortfolio.Stocks.Add(stockModel.Id.ToString());
            portfolioService.Update(userPortfolio);
            return RedirectToPage("/Portfolio");
        }


        public IActionResult OnPostSellRequest(PortfolioService portfolioService, StockService stockService)
        {
            var userName = Request.Cookies["username"];
            var dataClose = Convert.ToDouble(DataClose.Replace(".", ","));

            var UserService = new UserService();
            var user = UserService.GetOneByUsername(userName);

            var userPortfolio = portfolioService.GetOneByUserId(user.Id);
            if (userPortfolio != null)
            {
                foreach (var stockId in userPortfolio.Stocks)
                {
                    var stock = stockService.GetOneById(new ObjectId(stockId));

                    if (stock.StockCode == StockCode)
                    {
                        if (Amount <= stock.Amount) // Check if the user has enough shares to sell
                        {
                            var sellAmount = Amount;
                            var sellValue = Convert.ToDouble(dataClose) * sellAmount;
                            var remainingAmount = stock.Amount - sellAmount;

                            if (remainingAmount > 0)
                            {
                                // Calculate new average cost after selling
                                stock.AverageCost = (stock.AverageCost * stock.Amount - sellValue) / remainingAmount;
                                stock.Amount = remainingAmount;
                                stockService.Update(stock);
                            }
                            else
                            {
                                // Remove stock from portfolio if all shares are sold
                                stockService.Delete(stock.Id);
                                userPortfolio.Stocks.Remove(stockId);
                                portfolioService.Update(userPortfolio);
                            }

                            return Page();
                        }
                        else
                        {
                            // Display an error message or handle insufficient shares scenario
                            // For example, you can add a ModelState error and display it on the page.
                            ModelState.AddModelError("", "Insufficient shares to sell.");
                            return Page();
                        }
                    }
                }

                // Display an error message or handle stock not found scenario
                // For example, you can add a ModelState error and display it on the page.
                ModelState.AddModelError("", "Stock not found.");
                return Page();
            }

            return Page();
        }



        public void OnGet()
        {

        }
    }
}