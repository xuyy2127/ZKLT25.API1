using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZKLT25.API.Helper
{
    public class BasePageParams
    {
        private int _pageSize = 20;
        const int maxPageSize = 99999;
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                if (value >= 1)
                {
                    _pageSize = (value > maxPageSize) ? maxPageSize : value;
                }
            }
        }
        private int _pageNumber = 1;
        public int PageNumber
        {
            get
            {
                return _pageNumber;
            }
            set
            {
                if (value >= 1)
                {
                    _pageNumber = value;
                }
            }
        }
        public bool NoPaging { get; set; } = false;
    }

}
