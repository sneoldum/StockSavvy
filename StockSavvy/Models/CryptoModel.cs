using ThirdParty.Json.LitJson;

namespace StockSavvy.Models;

public class CryptoModel
{
    [JsonProperty]
    public string price { get; set; }
}