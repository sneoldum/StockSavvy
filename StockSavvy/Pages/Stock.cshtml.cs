using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockSavvy.Services;
using static StockSavvy.Models.StockModel;

namespace StockSavvy.Pages
{
    public class StockModel : PageModel
    {
        private readonly ILogger<StockModel> _logger;

        public StockModel(ILogger<StockModel> logger)
        {
            _logger = logger;
        }
        [BindProperty]
        public string StockCode { get; set; }

        [BindProperty]
        public string DataOpen { get; set; }

        [BindProperty]
        public string DataClose { get; set; }

        [BindProperty]
        public string DataHigh { get; set; }

        [BindProperty]
        public string DataLow { get; set; }

        [BindProperty]
        public string DataVolume { get; set; }

        [BindProperty]
        public string DataSymbol { get; set; }

        [BindProperty]
        public string DataLast { get; set; }

        [BindProperty]
        public List<Data> DataList { get; set; }
        [BindProperty]
        public List<string> Date { get; set; }

        [BindProperty]
        public string key { get; set; }


        public void OnPostStockRequest(StockService stockService)

        {
            Console.WriteLine(StockCode);
            var stockModel = stockService.GetStockModel(StockCode,key);
            if (stockModel != null)
            {
                var firstOrDefaultValue = stockModel.Datas.Values.FirstOrDefault();
                Date = stockModel.Datas.Keys.ToList();
                DataList =  stockModel.Datas.Values.ToList();
                
                DataSymbol = stockModel.metaData.Symbol;
                DataLast = stockModel.metaData.LastRefreshed;
                DataOpen = firstOrDefaultValue.Open;
                DataClose = firstOrDefaultValue.Close;
                DataHigh = firstOrDefaultValue.High;
                DataLow = firstOrDefaultValue.Low;
                DataVolume = firstOrDefaultValue.Volume;
            }

        }
        public void OnGet()
        {

        }
    }
}