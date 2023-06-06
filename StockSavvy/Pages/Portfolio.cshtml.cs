using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
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

        public void OnGet()
        {
            stocks = new List<StockMongoModel>();

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

        public int GetStockPrice(string symbol)
        {
            string QUERY_URL = "https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol="+symbol+"&apikey=asdasd";
            Uri queryUri = new Uri(QUERY_URL);

            using (WebClient client = new WebClient())
            {
                dynamic json_data = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(client.DownloadString(queryUri));
                return json_data["Global Quote"]["05. price"];
            }
        }
    }
}
