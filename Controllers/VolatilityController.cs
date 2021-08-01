using Grpc.Net.Client;
using IexService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QuandlService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TraderJoe.Enums;
using TraderJoe.Extensions;
using TraderJoe.Models;

namespace TraderJoe.Controllers
{
    public class VolatilityController : Controller
    {
        private readonly ILogger<VolatilityController> _logger;

        public VolatilityController(ILogger<VolatilityController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Quandl.QuandlClient(channel);
            var reply = await client.GetOpenPricesAsync(new OpenPricesRequest { Company = "MSFT", StartDate = "2017-11-26", EndDate = "2017-12-26" });
            //new method in IEX that gets the stock for a company over the last month

            var listOfCompanies = new List<string>() { CompanyAlias.AmericanExpressCompany, CompanyAlias.Boeing, CompanyAlias.CocaCola, CompanyAlias.Disney, CompanyAlias.ExxonMobilCorporation, CompanyAlias.GeneralElectricCompany, CompanyAlias.GoldmanSachsGroup, CompanyAlias.HomeDepot, CompanyAlias.IntelCorporation, CompanyAlias.InternationalBusinessMachines, CompanyAlias.JohnsonAndJohnson, CompanyAlias.JPMorganChase, CompanyAlias.McDonalds, CompanyAlias.Microsoft, CompanyAlias.Nike, CompanyAlias.Pfizer, CompanyAlias.ProcterAndGamble, CompanyAlias.ThreeMCompany, CompanyAlias.UnitedHealthGroupInc, CompanyAlias.Visa };

            Dictionary<string, double> CompanyVolatility = new Dictionary<string, double>();

            foreach (var company in listOfCompanies)
            {
                var timeSeriesData = await client.GetOpenPricesAsync(new OpenPricesRequest { Company = company, StartDate = "2017-11-26", EndDate = "2017-12-26" });

                var timeSeries = timeSeriesData.ToTimeSeries();

                List<TimeSeries> percentageDiff = PastMonthVolatility(timeSeries);

                double averageVolatility = Average(percentageDiff.Select(r => r.Value).ToList());

                CompanyVolatility[company] = averageVolatility;
            }


            return View();
        }

        private List<TimeSeries> PastMonthVolatility(List<TimeSeries> stockPricesByDay)
        {
            var dailyPercentageChange = new List<TimeSeries>();

            for(int i = 1; i < stockPricesByDay.Count; i++)
            {
                dailyPercentageChange.Add(new TimeSeries { Date = stockPricesByDay[i].Date, Value = PercentageChange(stockPricesByDay[i].Value, stockPricesByDay[i - 1].Value )});
            }

            return dailyPercentageChange;
        }

        private double PercentageChange(double todaysValue, double yesterdaysValue)
        {
            return todaysValue / yesterdaysValue * 100;
        }

        private double Average(List<Double> pcs) => pcs.Sum() / pcs.Count;
    }
}
