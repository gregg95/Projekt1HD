using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt1HD.Models
{
    public class Database_Review
    {

        public int Review_ID { get; set; }
        public int Item_ID { get; set; }
        public string Advantages { get; set; }
        public string Defects { get; set; }
        public string Review_summary { get; set; }
        public string Rating { get; set; }
        public string Reviewer { get; set; }
        public DateTime? Review_date { get; set; }
        public bool? Product_recommend { get; set; }
        public int? Votes_up { get; set; }
        public int? Votes_down { get; set; }
        public int? Review_CeneoID { get; set; }
        public string Review { get; set; }

    }
}
