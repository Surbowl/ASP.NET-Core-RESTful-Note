using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Routine.APi.Helpers
{
    public static class IEnumerableExtensions
    {
        //IEnumerable<T> 的拓展方法
        //用于查询集合时的数据塑形（视频P39）
        //分开两个拓展方法是出于性能的考虑
        //数据塑形可以考虑使用微软的 OData 规范及其相关框架 https://www.odata.org/
        public static IEnumerable<ExpandoObject> ShapeData<TSource>(this IEnumerable<TSource> source,
                                                                    string fields)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var expandoObjectList = new List<ExpandoObject>(source.Count());
            var propertyInfoList = new List<PropertyInfo>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                var propertyInfos = typeof(TSource)
                                    .GetProperties(BindingFlags.Public 
                                    | BindingFlags.Instance);
                propertyInfoList.AddRange(propertyInfos);
            }
            else
            {
                var fieldsAfterSplit = fields.Split(",");
                foreach (var field in fieldsAfterSplit)
                {
                    var propertyName = field.Trim();
                    var propertyInfo = typeof(TSource)
                                       .GetProperty(propertyName, 
                                                   BindingFlags.IgnoreCase //IgnoreCase 忽略大小写
                                                   | BindingFlags.Public 
                                                   | BindingFlags.Instance);
                    if (propertyInfo == null)
                    {
                        throw new Exception($"Property:{propertyName} 没有找到：{typeof(TSource)}");
                    }

                    propertyInfoList.Add(propertyInfo);
                }
            }

            foreach (TSource obj in source)
            {
                var shapedObj = new ExpandoObject();

                foreach (var propertyInfo in propertyInfoList)
                {
                    var propertyValue = propertyInfo.GetValue(obj);
                    ((IDictionary<string, object>)shapedObj).Add(propertyInfo.Name, propertyValue);
                }

                expandoObjectList.Add(shapedObj);
            }

            return expandoObjectList;
        }
    }
}
