using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt1HD.Models
{
    public class Product
    {
        public string Name { get; set; }
        public Uri Url { get; set; }
        public string ProductID { get; set; }
        public string ReviewsCount { get; set; }
    }
}
