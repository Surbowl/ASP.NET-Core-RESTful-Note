using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Routine.APi.Services
{
    //映射关系（视频P37）
    //TSource 是源类型，例如 EmployeeDto
    //TDestination 是目标类型，例如 Employee
    public class PropertyMapping<TSource, TDestination> : IPropertyMapping
    {
        //从 TSource 到 TDestination 的映射关系字典
        public Dictionary<string, PropertyMappingValue> MappingDictionary { get; private set; }
        public PropertyMapping(Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            MappingDictionary = mappingDictionary ?? throw new ArgumentNullException(nameof(mappingDictionary));
        }
    }
}
