using AutoMapper;
using Routine.APi.Entities;
using Routine.APi.Models;

/// <summary>
/// AutoMapper 配置文件
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

            //手动映射举例，把 Company(src) 的 Name 映射到 CompanyDto(dest) 的 Name
            //CreateMap<Company, CompanyDto>().ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
        }
    }
}
