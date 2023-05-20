using System.Globalization;
using System.Net;
using Newtonsoft.Json;
using StockSavvy.Models;
using static StockSavvy.Models.StockModel;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace StockSavvy.Services
{
    public class StockService
    {

        public StockModel? GetStockModel(string stockCode,string apikey)
        {
            if (apikey == null)
            {
                apikey = "demo"; // Your api key 
                stockCode = "IBM";
            }
            string url = string.Concat("https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY&symbol=" + stockCode + "&interval=5min&apikey=" + apikey);


            //string url = string.Concat("https://www.alphavantage.co/query?function=TIME_SERIES_WEEKLY&symbol=" + stockCode+"&apikey=", apikey);

            var json = new WebClient().DownloadString(url);
            StockModel? stockData = JsonSerializer.Deserialize<StockModel>(json);
            stockData.json = json;
            Console.WriteLine(stockData.Datas);
            return stockData;

        }
    }
}
