using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Helpers
{
    public class PaginationParams
    {
        private const int MaxPageSize = Int32.MaxValue;
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 8;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;

        }
    }
}
