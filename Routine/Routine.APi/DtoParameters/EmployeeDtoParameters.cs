using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Routine.APi.DtoParameters
{
    //（视频P36）
    public class EmployeeDtoParameters
    {
        private const int MaxPageSize = 20;
        public string GenderDisplay { get; set; }
        public string Q { get; set; }
        public int PageNumber { get; set; }
        private int _pageSize = 5;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        //排序依据（默认用 Name，应该使用 Dto 中的命名方式）
        public string OrderBy { get; set; } = "Name";
    }
}
