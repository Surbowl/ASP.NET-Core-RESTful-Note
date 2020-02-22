using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Routine.APi.Models
{
    //查询、插入、更新应该使用不同的Dto，便于业务升级与重构

    /// <summary>
    /// Create Company 时使用的 Dto，不含 BankruptTime 属性
    /// </summary>
    public class CompanyAddDto
    {
        [Display(Name = "公司名")]
        [Required(ErrorMessage = "{0}这个字段是必填的")]
        [MaxLength(100, ErrorMessage = "{0}的最大长度不可以超过{1}")]
        public string Name { get; set; }  //请注意，此处的属性名为 Name ，与视频中的 CompanyName 不同

        [Display(Name = "国家")]
        [Required(ErrorMessage = "{0}这个字段是必填的")]
        [MaxLength(100, ErrorMessage = "{0}的最大长度不可以超过{1}")]
        public string Country { get; set; }

        [Display(Name = "行业")]
        [MaxLength(100, ErrorMessage = "{0}的最大长度不可以超过{1}")]
        public string Industry { get; set; }

        [Display(Name = "产品")]
        [MaxLength(100, ErrorMessage = "{0}的最大长度不可以超过{1}")]
        public string Product { get; set; }

        [Display(Name = "简介")]
        [StringLength(500,MinimumLength =10,ErrorMessage = "{0}的长度范围从{2}到{1}")]
        //[MaxLength(500, ErrorMessage = "{0}的最大长度不可以超过{1}")]
        //[MinLength(10, ErrorMessage = "{0}的长度至少{1}位")]
        public string Introduction { get; set; }

        //这种写法可以避免空引用异常
        public ICollection<EmployeeAddDto> Employees { get; set; } = new List<EmployeeAddDto>();
    }
}

/*
 * 推荐使用第三方库 FluentValidation
 * - 容易创建复杂的验证规则
 * - 验证规则与 Model 分离
 * - 容易进行单元测试
 */
