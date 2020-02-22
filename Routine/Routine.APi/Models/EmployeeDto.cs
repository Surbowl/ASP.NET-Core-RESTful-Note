using System;

namespace Routine.APi.Models
{
    /// <summary>
    /// 输出 Employee 使用的 Dto，包含 Employee 的所有属性/字段，不区分 Full 与 Friendly
    /// </summary>
    public class EmployeeDto
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string EmployeeNo { get; set; }
        public string Name { get; set; }
        public string GenderDisplay { get; set; }

        public int Age { get; set; }
    }
}
