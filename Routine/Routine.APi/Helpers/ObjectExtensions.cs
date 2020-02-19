﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace Routine.APi.Helpers
{
    //Object 的拓展方法
    //用于查询单个数据时的数据塑形（视频P39）
    public static class ObjectExtensions
    {
        public static ExpandoObject ShapeData<TSource>(this TSource source, string fields)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            //理解 ExpandoObject 的使用 https://blog.csdn.net/u010178308/article/details/79773704
            var expandoObj = new ExpandoObject();

            //如果没有 fields 字符串指定属性，返回所有属性
            if (string.IsNullOrWhiteSpace(fields))
            {
                //获得 TSource 的属性
                var propertyInfos = typeof(TSource).GetProperties(BindingFlags.IgnoreCase  //忽略属性名大小写
                                                                  | BindingFlags.Public    //搜索公共成员
                                                                  | BindingFlags.Instance);
                foreach (var propertyInfo in propertyInfos)
                {
                    var propertyValue = propertyInfo.GetValue(source);
                    ((IDictionary<string, object>)expandoObj).Add(propertyInfo.Name, propertyValue);
                }
            }
            //如果有 fields 字符串指定属性，返回指定的属性
            else
            {
                var fieldsAfterSpilt = fields.Split(",");
                foreach (var field in fieldsAfterSpilt)
                {
                    var propertyName = field.Trim();
                    //获得 fields 字符串指定的 TSource 中的各属性
                    var propertyInfo = typeof(TSource).GetProperty(propertyName,
                                                                   BindingFlags.IgnoreCase  //忽略属性名大小写
                                                                  | BindingFlags.Public     //搜索公共成员
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