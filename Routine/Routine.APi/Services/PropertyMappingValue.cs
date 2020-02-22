using System;
using System.Collections.Generic;

namespace Routine.APi.Services
{
    /// <summary>
    /// 单条属性映射关系，用于集合资源的排序（视频P37）
    /// </summary>
    public class PropertyMappingValue
    {
        public IEnumerable<string> DestinationProperties { get; set; }

        /// <summary>
        /// 顺序是否需要翻转
        /// </summary>
        public bool Revert { get; set; }

        public PropertyMappingValue(IEnumerable<string> destinaryProperties, bool revert = false)
        {
            DestinationProperties = destinaryProperties ?? throw new ArgumentNullException(nameof(destinaryProperties));
            Revert = revert;
        }
    }
}
