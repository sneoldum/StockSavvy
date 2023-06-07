using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson;
using StockSavvy.Models;
using StockSavvy.Services;

namespace StockSavvy.Pages
{
	public class PortfolioModel : PageModel
    {
        [BindProperty]
        public List<StockMongoModel> stocks{ get; set; }
        public List<StockPriceModel> prices { get; set; }

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


        public void OnGet()
        {
            stocks = new List<StockMongoModel>();
            prices = new List<StockPriceModel>();

            var userName = Request.Cookies["username"];
            

            var UserService = new UserService();
            var user = UserService.GetOneByUsername(userName);

            var PortfolioService = new PortfolioService();
            var portfolio = PortfolioService.GetOneByUserId(user.Id);
            if (portfolio != null)
            {
                var stockIds = portfolio.Stocks;
                var StockService = new StockService();
                foreach (var id in stockIds)
                {
                    var stock = StockService.GetOneById(new ObjectId(id));
                    stocks.Add(stock);
                }
            }
        }
        
        private static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            char[] stringChars = new char[length];

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(stringChars);
        }
        
        public decimal GetStockPrice(string symbol)
        {
            string QUERY_URL = "https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol="+"IBM"+"&apikey=demo";
            Uri queryUri = new Uri(QUERY_URL);

            using (WebClient client = new WebClient())
            {
                var json_data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(client.DownloadString(queryUri));
                var data = json_data["Global Quote"];
                string priceString = data.GetProperty("05. price").GetString().Replace(".", ",");
                if (decimal.TryParse(priceString, out decimal price))
                {
                    StockPriceModel stockPrice = new StockPriceModel();
                    stockPrice.StockCode = symbol;
                    stockPrice.Price = Convert.ToSingle(price);
                    prices.Add(stockPrice);
                    return price;
                }
                else
                {
                    return -1;
                }
            }
        }

        public string GetNameOfStock(string symbol)
        {
            string QUERY_URL = "https://www.alphavantage.co/query?function=SYMBOL_SEARCH&keywords=" + symbol +
                               "&apikey=" + RandomString(5);
            Uri queryUri = new Uri(QUERY_URL);

            using (WebClient client = new WebClient())
            {
                var json_data =
                    JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(client.DownloadString(queryUri));
                var data = json_data["bestMatches"];
                var first = data.EnumerateArray().First();
                string name = first.GetProperty("2. name").GetString();
                name = name.Split(" ")[0];
                Console.WriteLine(name);
                return name.ToLower();
            }
        }
    }
}
