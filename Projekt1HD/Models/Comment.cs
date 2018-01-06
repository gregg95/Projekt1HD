using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt1HD.Models
{
    public class Comment
    {
        public string Commentator { get; set; }
        public string CommentString { get; set; }
        public List<CommentReply> Replies { get; set; }
    }
}
