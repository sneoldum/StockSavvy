using System.Net;
using System.Text.Json;
using MongoDB.Bson;
using MongoDB.Driver;
using StockSavvy.Models;

namespace StockSavvy.Services
{
    public class StockService
    {
        private IMongoCollection<StockMongoModel> StockCollection;

        public StockService()
        {
            var client =
                new MongoClient(
                    "mongodb+srv://admin:hQSH0VkQDAmU9gT2@mongocluster.uqjdact.mongodb.net/?retryWrites=true&w=majority");
            var database = client.GetDatabase("stocksavvy");
            StockCollection = database.GetCollection<StockMongoModel>("Stock");
        }
        HashSet<string> stockCodes = new HashSet<string>();

        
        public StockModel? GetStockModel(string stockCode, string apikey)
        {
            if (apikey == null)
            {
                apikey = "demo"; // Your api key 
                stockCode = "IBM";
            }
            string url = string.Concat("https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY&symbol=" +
                                       stockCode + "&interval=5min&apikey=" + apikey);
            //string url = string.Concat("https://www.alphavantage.co/query?function=TIME_SERIES_WEEKLY&symbol=" + stockCode+"&apikey=", apikey);

            var json = new WebClient().DownloadString(url);
            StockModel? stockData = JsonSerializer.Deserialize<StockModel>(json);
            stockData.json = json;
            Console.WriteLine(stockData.Datas);
            return stockData;
        }
        
        public CryptoModel? GetCryptoModel(string crypto)
        {
            if (crypto == null)
            {
                crypto = "ETH";
            }
            string url = string.Concat("https://api.binance.com/api/v3/ticker/price?symbol="+crypto+"USDT");
            //string url = string.Concat("https://www.alphavantage.co/query?function=TIME_SERIES_WEEKLY&symbol=" + stockCode+"&apikey=", apikey);

            var json = new WebClient().DownloadString(url);
            CryptoModel? cryptoData = JsonSerializer.Deserialize<CryptoModel>(json);
            return cryptoData;
        }

        //returns all of the Stocks from db
        public List<StockMongoModel> Get()
        {
            return StockCollection.Find(model => true).ToList();
        }

        //returns user by unique id
        public StockMongoModel GetOneById(ObjectId objectId)
        {
            return StockCollection.Find(model => model.Id == objectId).FirstOrDefault();
        }

        //creates a stock from scratch, Id field must be null in order to create new one otherwise it will try to update an existing one
        public string Create(StockMongoModel stockMongoModel)
        {
            StockCollection.InsertOne(stockMongoModel);
            return stockMongoModel.Id.ToString();
        }

        //updates a stock, stockmongomodel has to contain a valid stock id
        public string Update(StockMongoModel stockMongoModel)
        {
            StockCollection.ReplaceOne(model => model.Id == stockMongoModel.Id, stockMongoModel);
            return stockMongoModel.Id.ToString();
        }

        //deletes user by its id
        public void Delete(ObjectId objectId)
        {
            StockCollection.DeleteOne(model => model.Id == objectId);
        }
    }
}