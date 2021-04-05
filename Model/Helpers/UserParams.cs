using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Helpers
{
    public class UserParams : PaginationParams
    {

        public string CurrentUsername { get; set; }
        public string Gender { get; set; }
        public int MinAge { get; set; } = 18;
        public int MaxAge { get; set; } = 125;

        public string OrderBy { get; set; } = "lastActive";
    }
}
