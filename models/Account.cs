using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Transactions;

namespace ATMSimulator.models
{
    public class Account
    {
        public int id { get; set; }
        public string accountNumber { get; set; }
        public decimal balance { get; set; }
        public string type { get; set; }
        public string currency { get; set; }
        public DateTime openingDate { get; set; }
        public string status { get; set; }
        public List<object> operations { get; set; }
        public List<object> fromTransactions { get; set; }
        public List<object> toTransactions { get; set; }
    }
}
