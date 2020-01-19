using Routine.APi.Models;
using System.ComponentModel.DataAnnotations;

namespace Routine.APi.ValidationAttributes
{
    public class EmployeeNoMustDifferentFromFirstNameAttribute : ValidationAttribute
    {
        //作用于属性时 value 是属性值，作用于类时 value 是对象
        //validationContext 是属性所在的对象
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //var employeeAddDto = (EmployeeAddDto)value
            var employeeAddDto = (EmployeeAddDto)validationContext.ObjectInstance;
            if (employeeAddDto.EmployeeNo == employeeAddDto.FirstName)
            {
                return new ValidationResult("员工编号不可以等于名", new[] { nameof(EmployeeAddDto) });
            }
            return ValidationResult.Success;
        }
    }
}
