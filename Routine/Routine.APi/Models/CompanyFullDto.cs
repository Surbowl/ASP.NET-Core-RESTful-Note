using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Routine.APi.Models
{
    public class CompanyFullDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } //请注意，此处的属性名为 Name ，与视频中的 CompanyName 不同
        public string Country { get; set; }
        public string Industry { get; set; }
        public string Product { get; set; }
        public string Introduction { get; set; }
    }
}
