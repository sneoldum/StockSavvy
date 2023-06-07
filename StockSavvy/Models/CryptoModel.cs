using ThirdParty.Json.LitJson;

namespace StockSavvy.Models;

public class CryptoModel
{
    [JsonProperty]
    public string price { get; set; }
    public string name { get; set; }
    public class CryptoData
    {
        public string Close { get; set; }
        public string Time { get; set; }
    }

}