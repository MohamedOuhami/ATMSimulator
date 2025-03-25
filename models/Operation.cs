using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMSimulator.models
{
    public class Operation
    {
        public int id { get; set; }
        public float amount { get; set; }
        public DateTime timestamp { get; set; }
        public string type { get; set; }

    }
}
