using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Routine.APi.Helpers
{
    //Object 的拓展方法
    //用于查询单个数据时的数据塑形（视频P39）
    public static class ObjectExtensions
    {
        public static ExpandoObject ShapeData<TSource>(this TSource source,string fields)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var expandoObj = new ExpandoObject();

            if (string.IsNullOrWhiteSpace(fields))
            {
                var propertyInfos = typeof(TSource).GetProperties(BindingFlags.IgnoreCase 
                                                                  | BindingFlags.Public 
                                                                  | BindingFlags.Instance);
                foreach(var propertyInfo in propertyInfos)
                {
                    var propertyValue = propertyInfo.GetValue(source);
                    ((IDictionary<string, object>)expandoObj).Add(propertyInfo.Name, propertyValue);
                }
            }
            else
            {
                var fieldsAfterSpilt = fields.Split(",");
                foreach(var field in fieldsAfterSpilt)
                {
                    var propertyName = field.Trim();
                    var propertyInfo = typeof(TSource).GetProperty(propertyName,
                                                                   BindingFlags.IgnoreCase
                                                                  | BindingFlags.Public
                                                                  | BindingFlags.Instance);
                    if (propertyInfo == null)
                    {
                        throw new Exception($"在{typeof(TSource)}上没有找到{propertyName}这个属性");
                    }

                    var propertyValue = propertyInfo.GetValue(source);
                    ((IDictionary<string, object>)expandoObj).Add(propertyInfo.Name, propertyValue);
                }
            }

            return expandoObj;
        }
    }
}
