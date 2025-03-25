using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ATMSimulator.models
{

    public class Card
    {
        public int id { get; set; }
        public string cardNumber { get; set; }
        public DateTime expiryDate { get; set; }
        public string Type { get; set; }
        public string pin { get; set; }
        public string cvv { get; set; }
        public string status { get; set; }
        public List<Account> accounts { get; set; }
    }
}
