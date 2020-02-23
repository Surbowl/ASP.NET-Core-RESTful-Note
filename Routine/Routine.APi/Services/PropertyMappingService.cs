using Routine.APi.Entities;
using Routine.APi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Routine.APi.Services
{
    /// <summary>
    /// 对集合资源进行排序时的属性映射服务（视频P37）
    /// </summary>
    public class PropertyMappingService : IPropertyMappingService
    {
        //定义属性映射关系字典

        //Company Friendly Dto 的属性映射关系字典
        //GetCompanies 时，无论请求的是 Full Dto 还是 Friendly Dto，都允许按照 Full Dto 中的属性进行排序，所以这个字典用不上了
        //private readonly Dictionary<string, PropertyMappingValue> _companyPropertyMapping
        //    = new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase) //Key 大小写不敏感
        //    {
        //        {"Id",new PropertyMappingValue(new List<string>{"Id"}) },
        //        {"Name",new PropertyMappingValue(new List<string>{"Name"}) }
        //    };

        //Company Full Dto 的属性映射关系字典
        private readonly Dictionary<string, PropertyMappingValue> _companyFullPropertyMapping
            = new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase) //Key 大小写不敏感
            {
                {"Id",new PropertyMappingValue(new List<string>{"Id"}) },
                {"Name",new PropertyMappingValue(new List<string>{"Name"}) },
                {"Country",new PropertyMappingValue(new List<string>{"Country"}) },
                {"Industry",new PropertyMappingValue(new List<string>{"Industry"}) },
                {"Product",new PropertyMappingValue(new List<string>{"Product"}) },
                {"Introduction",new PropertyMappingValue(new List<string>{"Introduction"}) },
                {"BankruptTime",new PropertyMappingValue(new List<string>{"BankruptTime"}) },
            };

        //Employee Dto 的属性映射关系字典
        private readonly Dictionary<string, PropertyMappingValue> _employeeDtoPropertyMapping
            = new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase) //Key 大小写不敏感
            {
                {"Id",new PropertyMappingValue(new List<string>{"Id"}) },
                {"CompanyId",new PropertyMappingValue(new List<string>{"CompanyId"}) },
                {"EmployeeNo",new PropertyMappingValue(new List<string>{"EmployeeNo"}) },
                {"Name",new PropertyMappingValue(new List<string>{"FirstName","LastName"}) },//"Name" 对应 FirstName 与 LastName 两个属性
                {"GenderDisplay",new PropertyMappingValue(new List<string>{"Gender"}) },
                {"Age",new PropertyMappingValue(new List<string>{"DateOfBirth"}, true) }     //"Age" 对应 DateOfBirth 属性，并且要翻转顺序
            };


        //因为不能在 IList<T> 中直接使用泛型了，无法解析 IList<PropertyMapping<TSource, TDestination>>
        //所有需要定义一个接口 IPropertyMapping，让 PropertyMapping 实现这个接口
        //然后使用 IList<IPropertyMapping> 来实现
        /// <summary>
        /// “指明 TSource 与 TDestination 的属性映射关系字典”的列表
        /// </summary>
        private IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            //向列表中添加属性映射关系字典，同时指明该字典对应的源类型与目标类型
            //即向列表中添加“指明 TSource 与 TDestination 的属性映射关系字典”
            //_propertyMappings.Add(new PropertyMapping<CompanyFriendlyDto, Company>(_companyPropertyMapping));
            _propertyMappings.Add(new PropertyMapping<CompanyFullDto, Company>(_companyFullPropertyMapping));
            _propertyMappings.Add(new PropertyMapping<EmployeeDto, Employee>(_employeeDtoPropertyMapping));
        }

        /// <summary>
        /// 如果 TSource 与 TDestination 存在映射关系，返回属性映射关系字典
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <returns>从源类型到目标类型的属性映射关系</returns>
        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            var matchingMapping = _propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();
            var propertyMapping = matchingMapping.ToList();
            if (propertyMapping.Count == 1)
            {
                //如果 TSource 与 TDestination 存在映射关系，返回对应的属性映射关系字典
                return propertyMapping.First().MappingDictionary;
            }
            throw new Exception($"无法找到唯一的映射关系：{typeof(TSource)},{typeof(TDestination)}");
        }

        /// <summary>
        /// 判断 Uri Query 中的 orderBy 字符串是否合法（视频P38）
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <param name="orderBy">Uri Query 中的 orderBy 字符串，大小写不敏感</param>
        /// <returns>orderBy 字符串是否合法</returns>
        public bool ValidMappingExistsFor<TSource,TDestination>(string orderBy)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();
            if (string.IsNullOrWhiteSpace(orderBy))
            {
                return true;
            }
            var fieldAfterSplit = orderBy.Split(",");
            foreach(var field in fieldAfterSplit)
            {
                var trimedField = field.Trim();
                var indexOfFirstSpace = trimedField.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ? trimedField : trimedField.Remove(indexOfFirstSpace);
                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
