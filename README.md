# ASP.NET-Core-RESTful-Note

[![image](https://raw.githubusercontent.com/Surbowl/ASP.NET-Core-RESTful-Note/master/cover.jpg)](https://www.bilibili.com/video/av77957694)

本仓库是[杨旭](https://www.cnblogs.com/cgzl/)（solenovex）《[使用 ASP.NET Core 3.x 构建 RESTful Web API](https://www.bilibili.com/video/av77957694)》课程的学习笔记。
<br><br>
包含课程的项目代码，并注释随堂笔记与资料。
<br><br>
与原项目略有不同，本项目使用 SQL Server 数据库。

## 版本
`master` 分支是最新的，涵盖所有课程内容。
<br><br>
在寻找更早的版本？欢迎查看本仓库的 [Releases](https://github.com/Surbowl/ASP.NET-Core-RESTful-Note/releases)，在课程的每个阶段都有 Release；
<br>
例如：[截至视频 P8 的代码](https://github.com/Surbowl/ASP.NET-Core-RESTful-Note/releases/tag/P8)、 [截至视频 P19 的代码](https://github.com/Surbowl/ASP.NET-Core-RESTful-Note/releases/tag/P19) 等。
<br><br>
杨老师已在 GitHub 发布课程原版代码，[点此访问](https://github.com/solenovex/ASP.NET-Core-3.x-REST-API-Tutorial-Code)。

## PATH
```
│  appsettings.Development.json
│  appsettings.json
│  Program.cs
│  Routine.APi.csproj
│  Startup.cs
|
├─Controllers
│      RootController.cs                   // api                                  根目录
│      CompaniesController.cs              // api/companies                        公司（单个/集合）
│      CompanyCollectionsController.cs     // api/companycollections               指定公司集合
│      EmployeesController.cs              // api/companies/{companyId}/employees  员工（单个/集合）
│      
├─Data
│      RoutineDbContext.cs
│      
├─DtoParameters                            // Uri query parameters
│      CompanyDtoParameters.cs             //  -GET api/companies   
│      EmployeeDtoParameters.cs            //  -GET api/companies/{companyId}/employees
│      
├─Entities
│      Company.cs
│      Employee.cs
│      Gender.cs
│      
├─Helpers
│      ArrayModelBinder.cs                 // 自定义 ModelBinder，将 ids 字符串转为 IEnumerable<Guid>
│      IEnumerableExtensions.cs            // IEnumerable<T> 拓展，对集合资源进行数据塑形
│      IQueryableExtensions.cs             // IQueryable<T> 拓展，对集合资源进行排序
│      ObjectExtensions.cs                 // Object 拓展，对单个资源进行数据塑形
│      PagedList.cs                        // 继承 List<T>，对集合资源进行翻页处理
│      ResourceUriType.cs                  // 枚举，指明 Uri 前往上一页、下一页或本页
│      
├─Migrations
│      ...
│      
├─Models
│      CompanyFriendlyDto.cs               // 公司简略信息 Dto
│      CompanyFullDto.cs                   // 公司完整信息 Dto
│      CompanyAddDto.cs                    // 添加公司时使用的 Dto
│      CompanyAddWithBankruptTimeDto.cs    // 添加已破产的公司时使用的 Dto
│      EmployeeDto.cs                      // 员工信息 Dto
│      EmployeeAddOrUpdateDto.cs           // 添加或更新员工信息时使用的 Dto 的父类
│      EmployeeAddDto.cs                   // 添加员工时使用的 Dto，继承 EmployeeAddOrUpdateDto
│      EmployeeUpdateDto.cs                // 更新员工信息时使用的 Dto，继承 EmployeeAddOrUpdateDto
│      LinkDto.cs                          // HATEOAS 的 links 使用的 Dto
|     
├─Profiles                                 // AutoMapper 映射关系
│      CompanyProfile.cs
│      EmployeeProfile.cs
│      
├─Properties
│      launchSettings.json
│      
├─Services
│      ICompanyRepository.cs
│      IPropertyCheckerService.cs
│      IPropertyMapping.cs
│      IPropertyMappingService.cs
│      CompanyRepository.cs
│      PropertyCheckerService.cs           // 判断 Uri query parameters 中的 fields 是否合法
│      PropertyMappingValue.cs             // 属性映射关系，用于集合资源的排序
│      PropertyMapping.cs                  // 属性映射关系字典，声明源类型与目标类型，用于集合资源的排序
│      PropertyMappingService.cs           // 属性映射关系服务，用于集合资源的排序
│      
└─ValidationAttributes                     //自定义 Model 验证 Attribute
        EmployeeNoMustDifferentFromFirstNameAttribute.cs  
        
```

## 小地图
- [视频课程](https://www.bilibili.com/video/av77957694)
- [配套博文](https://www.cnblogs.com/cgzl/p/11814971.html)
- [ASP.NET Core 3.x 入门课程](https://www.bilibili.com/video/av65313713)
- [How to unapply a migration](https://stackoverflow.com/questions/38192450/how-to-unapply-a-migration-in-asp-net-core-with-ef-core)
- [码云仓库（强制同步）](https://gitee.com/surbowl/ASP.NET-Core-RESTful-Note)
<br><br>
欢迎大家对内容进行补充，只要是合理内容都可以 [pull](https://github.com/Surbowl/ASP.NET-Core-RESTful-Note/pulls)。
<br><br>
非常感谢杨老师 🤗
