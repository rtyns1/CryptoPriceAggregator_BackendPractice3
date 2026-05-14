using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace CryptoPriceAggregator_BackendPractice3.Models
{
    public class PriceData
    {
        // pure data containers here only, so getters and setters, no methods no logic
        public string? SourceAPI { get; set; }
        public string? Cryptocurrency { get; set; }
        public double PriceUSD { get; set; }
        public DateTime RetrievedAt { get; set; }
    }

}

