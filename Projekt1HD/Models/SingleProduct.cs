using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt1HD.Models
{
    public class SingleProduct
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string LowerPrice { get; set; }
        public string Rating { get; set; }
        public string VotesCount { get; set; }
        public string ReviewsCount { get; set; }
        public string Category { get; set; }
        public string Additional_Informations { get; set; }
        public List<Review> Reviews { get; set; }
    }
}
