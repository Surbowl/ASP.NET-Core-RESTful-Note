using Routine.APi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Routine.APi.Helpers
{
    //（视频P37）
    public static class IQueryableExtensions
    {
        //这是一个 IQueryable<T> 的拓展方法，接收排序字符串与属性映射字典，返回排序后的 IQueryable<T>
        public static IQueryable<T> ApplySort<T>(this IQueryable<T> source,
                                                 string orderBy,
                                                 Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (mappingDictionary == null)
            {
                throw new ArgumentNullException(nameof(mappingDictionary));
            }
            if (string.IsNullOrWhiteSpace(orderBy))
            {
                return source;
            }

            //将 orderBy 字符串按 ',' 划分为数组
            var orderByAfterSplit = orderBy.Split(",");
            //依次处理数组中的每个排序依据
            foreach (var orderByClause in orderByAfterSplit.Reverse()) //需要 Reverse() 反转
            {
                //去除首位空格
                var trimmedOrderByClause = orderByClause.Trim();
                //是否 DESC
                var orderDescending = trimmedOrderByClause.EndsWith(" desc");
                //第一个空格的 index
                var indexOfFirstSpace = trimmedOrderByClause.IndexOf(" ");
                //属性名
                //如果存在空格，移除空格后面的内容（用来移除" desc"）
                var propertyName = indexOfFirstSpace == -1 ? trimmedOrderByClause : trimmedOrderByClause.Remove(indexOfFirstSpace);

                //在属性映射字典中查找
                //属性映射字典的 Key 大小写不敏感，不用担心大小写问题
                if (!mappingDictionary.ContainsKey(propertyName))
                {
                    throw new ArgumentNullException($"没有找到Key为{propertyName}的映射");
                }

                var propertyMappingValue = mappingDictionary[propertyName];
                if (propertyMappingValue == null)
                {
                    throw new ArgumentNullException(nameof(propertyMappingValue));
                }

                foreach(var destinationProperty in propertyMappingValue.DestinationProperties.Reverse()) //需要 Reverse() 反转
                {
                    if (propertyMappingValue.Revert)
                    {
                        orderDescending = !orderDescending;
                    }

                    //需要安装 System.Linq.Dynamic.Core 包，才能使用以下代码
                    //System.Linq.Dynamic.Core 包中的 OrderBy 可以直接传入字符串构造 T-SQL 语句
                    //System.Linq.Dynamic.Core 包中的 OrderBy 可以直接叠加，不用 .OrderBy().ThenBy() 这种写法
                    //越后面被 OrderBy 的属性，权重越高，所以在书面的代码中使用了两次 Reverse() 反转
                    source = source.OrderBy(destinationProperty + (orderDescending ? " descending" : " ascending"));
                }
            }
            return source;
        }
    }
}
