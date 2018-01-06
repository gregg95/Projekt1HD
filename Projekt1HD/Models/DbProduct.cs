using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt1HD.Models
{
    public class DbProduct
    {
        public int Prd_ID { get; set; }
        public int Prd_CeneoID { get; set; }
        public string Prd_Type { get; set; }
        public string Prd_Brand { get; set; }
        public string Prd_Model { get; set; }
        public string Prd_Comments { get; set; }

    }
}
