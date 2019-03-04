using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProInterface.Models.Api
{
    public class ApiInsuranceExpenseBean
    {
        public string KINDCODE { get; set; }
        public string KINDNAME { get; set; }
        public decimal AMOUNT { get; set; }
        public decimal BENCHMARKPREMIUM { get; set; }
        public decimal DISCOUNT { get; set; }
        public decimal PREMIUM { get; set; }
        public decimal BASEPREMIUM { get; set; }
        public decimal RATE { get; set; }
    }
}
