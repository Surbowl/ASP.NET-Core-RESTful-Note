using System;

namespace Routine.APi.Models
{
    //输出使用的 Friendly Dto
    //从视频P43 Vendor-specific Media Types 开始，默认的 CompanyDto 不再包含全部信息
    //如需输出全部信息，需使用 CompanyFullDto
    public class CompanyDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } //请注意，此处的属性名为 Name ，与视频中的 CompanyName 不同
    }
}
