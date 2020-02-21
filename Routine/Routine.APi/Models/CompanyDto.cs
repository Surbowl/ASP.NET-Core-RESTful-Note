using System;

namespace Routine.APi.Models
{
    //输出使用的Dto
    public class CompanyDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } //请注意，此处的属性名为 Name ，与视频中的 CompanyName 不同
    }
}
