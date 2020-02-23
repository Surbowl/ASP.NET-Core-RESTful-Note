using Routine.APi.Models;
using System.ComponentModel.DataAnnotations;

namespace Routine.APi.ValidationAttributes
{
    /// <summary>
    /// 自定义验证 Attribute（视频P28）
    /// </summary>
    public class EmployeeNoMustDifferentFromFirstNameAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //当自定义验证 Attribute 作用于属性时 value 是属性值，作用于类时 value 是类对象
            //而 validationContext 始终是类对象
            //var employeeAddDto = (EmployeeAddDto)value;
            var employeeAddDto = (EmployeeAddOrUpdateDto)validationContext.ObjectInstance;

            //EmployeeNo 不能与 FirstName 相等
            if (employeeAddDto.EmployeeNo == employeeAddDto.FirstName)
            {
                return new ValidationResult(ErrorMessage, new[] { nameof(EmployeeAddOrUpdateDto) });
            }

            return ValidationResult.Success;
        }
    }
}
