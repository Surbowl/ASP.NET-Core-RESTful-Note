using Routine.APi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Routine.APi.Helpers
{
    
    public static class IQueryableExtensions //这并不是一个接口，这种命名方式有点问题
    {
        //这是一个 IQueryable<T> 的拓展方法，接收排序字符串与属性映射字典，返回排序后的 IQueryable<T>
        //关于拓展方法，可以参考杨老师的另一个视频课程 https://www.bilibili.com/video/av62661924?p=5
        //排序（视频P37）
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

            //空字符串用来储存 OrderBy T-SQL 命令
            string ordering = "";

            //将 orderBy 字符串按 ',' 划分为数组
            var orderByAfterSplit = orderBy.Split(",");
            //依次处理数组中的每个排序依据
            foreach (var orderByClause in orderByAfterSplit)
            {
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

                foreach(var destinationProperty in propertyMappingValue.DestinationProperties)
                {
                    if (propertyMappingValue.Revert)
                    {
                        orderDescending = !orderDescending;
                    }

                    //构造 order by T-SQL 命令
                    //与视频中的方法略有不同，这种方法不用 Revert() 两次，性能更好
                    if (ordering.Length > 0)
                    {
                        ordering += ",";
                    }
                    ordering += destinationProperty + (orderDescending ? " descending" : " ascending");
                }
            }

            //执行 order by T-SQL 命令
            //需要安装 System.Linq.Dynamic.Core 包，才能使用以下代码
            source = source.OrderBy(ordering);
            return source;
        }
    }
}
