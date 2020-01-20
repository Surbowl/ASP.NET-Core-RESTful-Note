using AutoMapper;
using Routine.APi.Entities;
using Routine.APi.Models;
using System;

/// <summary>
/// AutoMapper 映射关系 配置文件
/// </summary>
namespace Routine.APi.Profiles
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            CreateMap<Employee, EmployeeDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.GenderDisplay, opt => opt.MapFrom(src => src.Gender.ToString()))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => GetAge(src.DateOfBirth)));

            CreateMap<EmployeeAddDto, Employee>();

            CreateMap<EmployeeUpdateDto, Employee>();

            CreateMap<Employee, EmployeeUpdateDto>();
        }

        /// <summary>
        /// 获得年龄
        /// </summary>
        /// <param name="dateOfBirth">出生日期</param>
        /// <returns></returns>
        private int GetAge(DateTime dateOfBirth)
        {
            DateTime dateOfNow = DateTime.Now;
            if (dateOfBirth > dateOfNow)
            {
                throw new ArgumentOutOfRangeException(nameof(dateOfBirth));
            }
            int age = dateOfNow.Year - dateOfBirth.Year;
            if (dateOfNow.Month < dateOfBirth.Month)
            {
                age--;
            }
            else if (dateOfNow.Month == dateOfBirth.Month && dateOfNow.Day < dateOfBirth.Day)
            {
                age--;
            }
            return age;
        }
    }
}
