using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Routine.APi.Models
{
    //输入使用的Dto
    //查询、插入、更新应该使用不同的Dto，便于业务升级与重构
    public class CompanyAddDto
    {
        public string Name { get; set; }
        public string Introduction { get; set; }
    }
}
