using QuandlService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TraderJoe.Models;

namespace TraderJoe.Extensions
{
    public static class Extensions
    {
        public static List<TimeSeries> ToTimeSeries(this OpenPricesResponse response)
        {
            var timeSeries = new List<TimeSeries>();

            foreach (var price in response.OpenPrices)
            {
                timeSeries.Add(new TimeSeries { Date = Convert.ToDateTime(price.Date), Value = price.Price });
            }

            return timeSeries;
        }
    }
}
