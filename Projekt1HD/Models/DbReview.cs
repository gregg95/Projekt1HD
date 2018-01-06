using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt1HD.Models
{
    public class DbReview
    {

        public int Rev_ID { get; set; }
        public int Rev_CeneoID { get; set; }
        public int Rev_PrdID { get; set; }
        public string Rev_Advantages { get; set; }
        public string Rev_Defects { get; set; }
        public string Rev_Summary { get; set; }
        public string Rev_Rating { get; set; }
        public string Rev_Reviewer { get; set; }
        public DateTime? Rev_Date { get; set; }
        public string Rev_Recom { get; set; }
        public int Rev_UpVotes { get; set; }
        public int Rev_DownVotes { get; set; }
        public string Rev_Content { get; set; }
        public bool IsReviewInserted { get; set; }
    }
}
