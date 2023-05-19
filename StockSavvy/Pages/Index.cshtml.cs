using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Binance.Net;
using Binance.Net.Clients;
using System.Linq;
using Binance.Net.Interfaces;

namespace StockSavvy.Pages
{
    public class IndexModel : PageModel
    {
        private readonly BinanceClient binanceClient;

        public IndexModel()
        {
            binanceClient = new BinanceClient(); // Replace apiKey and apiSecret with your actual Binance API credentials
        }
        
        public List<IBinanceTick> tickers { get; private set; }

        public async Task OnGet()
        {
            var spotTickerData = await binanceClient.SpotApi.ExchangeData.GetTickersAsync();
            tickers = spotTickerData.Data.Where(ticker => ticker.Symbol.EndsWith("USDT")).OrderByDescending(ticker => ticker.Volume * ticker.LastPrice).Take(100).ToList();
        }
    }
}