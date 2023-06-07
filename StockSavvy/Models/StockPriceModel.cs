using System.Text.Json.Serialization;

namespace StockSavvy.Models;

public class StockPriceModel
{
    [JsonPropertyName("stockCode")]
    public string StockCode { get; set; }
    [JsonPropertyName("price")]
    public float Price { get; set; }
}