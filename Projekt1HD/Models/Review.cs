using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt1HD.Models
{
    public class Review
    {
        public string Reviewer { get; set; }
        public List<string> Advantages { get; set; }
        public List<string> Defects { get; set; }
        public List<Comment> Comments { get; set; }
        public string Product_Recommended { get; set; }
        public string Review_Text { get; set; }
        public string Review_ID { get; set; }
        public string Date { get; set; }
        public string Summary { get; set; }
        public string Rating { get; set; }
        public string Votes_Yes { get; set; }
        public string Votes_No { get; set; }
    }
}
