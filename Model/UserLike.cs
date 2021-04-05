using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class UserLike
    {
        public ApplicationUser UserLiking { get; set; }
        public string UserLikingId { get; set; }
        public ApplicationUser UserBeingLiked { get; set; }
        public string UserBeingLikedId { get; set; }
        public DateTime DateLiked { get; set; } = DateTime.Now;
    }
}
