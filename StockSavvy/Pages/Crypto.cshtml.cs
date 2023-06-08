using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson;
using StockSavvy.Models;
using StockSavvy.Services;
using static StockSavvy.Models.CryptoModel;

namespace StockSavvy.Pages
{
    public class CryptoModel : PageModel
    {
        private readonly ILogger<CryptoModel> _logger;

        [BindProperty]
        public string CryptoCode { get; set; }
        
        [BindProperty]
        public string Price { get; set; }
        
        [BindProperty]
        public int Amount { get; set; }
        [BindProperty]
        public List<CryptoData> CryptoDataList { get; set; }
        
        
        
        

        public CryptoModel(ILogger<CryptoModel> logger)
        {
            _logger = logger;
        }
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

        public void OnPostCryptoRequest(StockService stockService)
        {
            var stockModel = stockService.GetCryptoModel(CryptoCode);
            
            Price = stockModel.price;

        }
        
        public IActionResult OnPostBuyRequest(PortfolioService portfolioService, StockService stockService)
        {
            var userName = Request.Cookies["username"];
            var dataClose = Convert.ToDouble(Price.Replace(".", ","));
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
               

                if (stock.StockCode == CryptoCode)
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
                StockCode = CryptoCode,
                Status = true
            };

            stockService.Create(stockModel);
            userPortfolio.Stocks.Add(stockModel.Id.ToString());
            portfolioService.Update(userPortfolio);
            return RedirectToPage("/Portfolio");
        }
        public void OnGet()
        {
            CryptoCode = "BTC";
        }
    }
}