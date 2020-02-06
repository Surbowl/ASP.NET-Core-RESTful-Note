using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Routine.APi.Services
{
    //排序使用的属性映射值（视频P37）
    public class PropertyMappingValue
    {
        public IEnumerable<string> DestinationProperties { get; set; }

        //是否顺序反转
        public bool Revert { get; set; }
        public PropertyMappingValue(IEnumerable<string> destinaryProperties, bool revert = false)
        {
            DestinationProperties = destinaryProperties ?? throw new ArgumentNullException(nameof(destinaryProperties));
            Revert = revert;
        }
    }
}
