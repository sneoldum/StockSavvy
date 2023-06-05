using System;
using System.Collections.Generic;
using System.Linq;
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

            var userName = "hakan";

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
    }
}
