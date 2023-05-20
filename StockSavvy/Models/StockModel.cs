using Newtonsoft.Json;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StockSavvy.Models
{

    public class StockModel
    {
        public string json { get; set; }
        [JsonPropertyName("Meta Data")]
        public MetaData metaData { get; set; }



        // Open for TIME_SERIES_INTRADAY
        [JsonPropertyName("Time Series (5min)")]
        //-------------------------
        //[JsonPropertyName("Weekly Time Series")] 
        public Dictionary<string, Data> Datas { get; set; }
        
        public class MetaData
        {

            [JsonPropertyName("1. Information")]
            public string Information { get; set; }

            [JsonPropertyName("2. Symbol")]
            public string Symbol { get; set; }
            
            [JsonPropertyName("3. Last Refreshed")]
            public string LastRefreshed { get; set; }

            //Open for TIME_SERIES_INTRADAY

            [JsonPropertyName("4. Interval")]
            public string Interval { get; set; }

            [JsonPropertyName("5. Output Size")]
            public string OutputSize { get; set; }

            [JsonPropertyName("6. Time Zone")]
            //-------------------------------------
            //[JsonPropertyName("4. Time Zone")]
            public string TimeZone { get; set; }
        }

        public class Data
        {
            [JsonPropertyName("1. open")]
            public string Open { get; set; }
            
            [JsonPropertyName("2. high")]
            public string High { get; set; }
            
            [JsonPropertyName("3. low")]
            public string Low { get; set; }
            
            [JsonPropertyName("4. close")]
            public string Close { get; set; }
            
            [JsonPropertyName("5. volume")]
            public string Volume { get; set; }
        }


    }
}
