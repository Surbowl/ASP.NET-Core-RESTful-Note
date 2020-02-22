using System.Collections.Generic;

namespace Routine.APi.Services
{
    /// <summary>
    /// PropertyMappingService 的接口
    /// </summary>
    public interface IPropertyMappingService
    {
        Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>();
        bool ValidMappingExistsFor<TSource, TDestination>(string fields);
    }
}