using System;

namespace Routine.APi.Models
{
    /// <summary>
    /// 输出 Company 使用的 Full Dto，包含 Company 的所有属性/字段（视频P43）
    /// </summary>
    public class CompanyFullDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } //请注意，此处的属性名为 Name ，与视频中的 CompanyName 不同
        public string Country { get; set; }
        public string Industry { get; set; }
        public string Product { get; set; }
        public string Introduction { get; set; }
        public DateTime? BankruptTime { get; set; }
    }
}
