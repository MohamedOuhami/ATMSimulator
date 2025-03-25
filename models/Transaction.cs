using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMSimulator.models
{
    public class Transaction
    {
        public int id { get; set; }
        public float amount { get; set; }
        public DateTime timeStamp { get; set; }

        // From Account

        public Account? fromAccount { get; set; }

        public Account? toAccount { get; set; }
    }
}
