using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMSimulator.models
{
    public class WithdrawalResult
    {
        public bool Success { get; set; }
        public decimal NewBalance { get; set; }
    }


}
