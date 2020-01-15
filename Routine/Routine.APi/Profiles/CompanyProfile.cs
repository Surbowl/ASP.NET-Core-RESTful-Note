using AutoMapper;
using Routine.APi.Entities;
using Routine.APi.Models;

/// <summary>
/// AutoMapper 映射关系 配置文件
/// </summary>
namespace Routine.APi.Profiles
{
    public class CompanyProfile : Profile
    {
        public CompanyProfile()
        {
            //原类型Company -> 目标类型CompanyDto
            //AutoMapper 基于约定
            //属性名称一致时自动赋值
            //自动忽略空引用
            CreateMap<Company, CompanyDto>();
            CreateMap<CompanyAddDto, Company>();
        }
    }
}
