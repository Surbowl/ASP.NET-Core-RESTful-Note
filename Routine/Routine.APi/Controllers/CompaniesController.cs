using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Routine.APi.ActionConstraints;
using Routine.APi.DtoParameters;
using Routine.APi.Entities;
using Routine.APi.Helpers;
using Routine.APi.Models;
using Routine.APi.Services;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

/*
 * HTTP方法：                                  |   安全   幂等
 *                                             |
 * GET      - 查询                             |    Y     Y
 *                                            |
 * POST     - 创建/添加                        |     N     N
 *            服务器端负责 URI 的生成           |
 *                                            |
 * PATCH    - 局部修改/更新                    |    N     N
 *            请求的 MediaType 是              |
 *            application/json-patch+json     |
 *                                            |
 * PUT      - 如果存在就替换，不存在则创建       |     N     Y
 *            如果 URI 存在，就更新资源         | 
 *            如果 URI 不存在，就创建资源或     |
 *            返回404（可选）                  |
 *                                            |
 * DELETE   - 移除/删除                        |     N     Y
 *                                            |
 * OPTIONS  - 获取 Web API 的通信选项的信息     |     Y     Y
 *                                            |
 * HEAD     - 只请求页面的首部                  |     Y     Y
 * 
 * 安全性是指方法执行后并不会改变资源的表述
 * 幂等性是指方法无论执行多少次都会得到同样的结果
 */

/*
 * 返回状态码：
 * 
 * 2xx 成功
 * 200 - OK 请求成功
 * 201 - Created 请求成功并创建了资源
 * 204 - No Content 请求成功，但是不应该返回任何东西，例如删除操作
 * 
 * 4xx 客户端错误
 * 400 - Bad Request API消费者发送到服务器的请求是有错误的
 * 401 - Unauthorized 没有提供授权信息或者提供的授权信息不正确
 * 403 - Forbidden 身份认证已经成功，但是已认证的用户却无法访问请求的资源
 * 404 - Not Found 请求的资源不存在
 * 405 - Method Not Allowed 尝试发送请求到资源的时候，使用了不被支持的HTTP方法
 * 406 - Not Acceptable API消费者请求的表述格式并不被Web API所支持，并且API不
 *       会提供默认的表述格式
 * 409 - Conflict 请求与服务器当前状态冲突（通常指更新资源时发生的冲突）
 * 415 - Unsupported Media Type 与406正好相反，有一些请求必须带着数据发往服务器，
 *       这些数据都属于特定的媒体格式，如果API不支持该媒体类型格式，415就会被返回
 * 422 - Unprocessable Entity 它是HTTP拓展协议的一部分，它说明服务器已经懂得了
 *       Content Type，实体的语法也没有问题，但是服务器仍然无法处理这个实体数据 
 * 
 * 5xx 服务器错误
 * 500 - Internal Server Error 服务器出现错误
 */

/*
 * 绑定数据源：
 * [FromBody] 
 * [FromForm] 
 * [FromHeader]（略）
 * [FromQuery] 
 * [FromRoute] 
 * [FromService]（略）
 */

namespace Routine.APi.Controllers
{   
    /*[ApiController] 属性并不是强制要求的，但是它会使开发体验更好
     * 它会启用以下行为：
     * 1.要求使用属性路由（Attribute Routing）
     * 2.自动HTTP 400响应
     * 3.推断参数的绑定源
     * 4.Multipart/form-data请求推断
     * 5.错误状态代码的问题详细信息
     */
    [ApiController]
    [Route("api/companies")] //还可用 [Route("api/[controller]")]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IPropertyCheckerService _propertyCheckerService;

        public CompaniesController(ICompanyRepository companyRepository,
                                   IMapper mapper,
                                   IPropertyMappingService propertyMappingService,
                                   IPropertyCheckerService propertyCheckerService)
        {
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _propertyCheckerService = propertyCheckerService ?? throw new ArgumentNullException(nameof(propertyCheckerService));
        }

        #region Controllers
        #region HttpGet

        [HttpGet(Name = nameof(GetCompanies))]
        [HttpHead] //添加对 Http Head 的支持，Head 请求只会返回 Header 信息，没有 Body（视频P16）
        public async Task<IActionResult> GetCompanies([FromQuery]CompanyDtoParameters parameters,
                                                      [FromHeader(Name="Accept")]  //mediaType 从 Header 获取
                                                      string acceptMediaType)
        {
            //尝试解析 MediaTypeHeaderValue（视频P43）
            //关于 MediaTypeHeaderValue https://docs.microsoft.com/zh-cn/dotnet/api/system.net.http.headers.mediatypeheadervalue
            if (!MediaTypeHeaderValue.TryParse(acceptMediaType, out MediaTypeHeaderValue parsedAcceptMediaType))
            {
                return BadRequest();  //返回状态码400
            }

            //判断 Uri Query 中的 orderby 字符串是否合法（视频P38）
            //无论请求的是 Full Dto 还是 Friendly Dto，都允许按照 Full Dto 中的属性进行排序
            if (! _propertyMappingService.ValidMappingExistsFor<CompanyFullDto, Company>(parameters.OrderBy))
            {
                return BadRequest();  //返回状态码400
            }

            //判断 Uri Query 中的 fields 字符串是否合法（视频P39）
            if (! _propertyCheckerService.TypeHasProperties<CompanyDto>(parameters.Fields))
            {
                return BadRequest();
            }

            //GetCompaniesAsync(parameters) 返回的是经过翻页处理的 PagedList<T>（视频P35）
            var companies = await _companyRepository.GetCompaniesAsync(parameters);
            //向 Headers 中添加翻页信息
            var paginationMetdata = new
            {
                totalCount = companies.TotalCount,
                pageSize = companies.PageSize,
                currentPage = companies.CurrentPage,
                totalPages = companies.TotalPages
            };
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetdata,
                                                                          new JsonSerializerOptions
                                                                          {   //为了防止 URI 中的‘&’、‘？’符号被转义，使用“不安全”的 Encoder
                                                                              Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                                                                          }));           
            //是否需要 Full Dto（视频P43）
            bool isFull = parsedAcceptMediaType
                         .SubTypeWithoutSuffix
                         .ToString()
                         .Contains("full", StringComparison.InvariantCultureIgnoreCase);
            //是否需要 links（HATEOAS）（视频P41-43）
            bool includeLinks = parsedAcceptMediaType
                               .SubTypeWithoutSuffix
                               .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase); //忽略大小写

            var shapedData = isFull ?
                           _mapper.Map<IEnumerable<CompanyFullDto>>(companies).ShapeData(parameters.Fields)
                           :
                           _mapper.Map<IEnumerable<CompanyDto>>(companies).ShapeData(parameters.Fields);

            //使用 HATEOAS，返回 value 与 links （视频P43）
            if (includeLinks)
            {
                //首先创建 Companies 集合中每个 Company 自己的 Links
                var shapedCompaniesWithLinks = shapedData.Select(c =>
                {
                    var companyDict = c as IDictionary<string, object>;
                    var links = CreateLinksForCompany((Guid)companyDict["Id"], null);
                    companyDict.Add("links", links);
                    return companyDict;
                });

                //然后创建整个 Companies 集合的 Links
                var linkedCollectionResource = new
                {
                    value = shapedCompaniesWithLinks,
                    links = CreateLinksForCompany(parameters, companies.HasPrevious, companies.HasNext)
                };
                return Ok(linkedCollectionResource);  //返回状态码200
            }

            //不使用 HATEOAS，只返回 value
            return Ok(shapedData);
        }

        [HttpGet("{companyId}", Name = nameof(GetCompany))]  //可省略 [Route("{companyId}")]

        //本项目在 Startup.cs 中对输出格式化器进行全局设置，不再使用 Produces 属性进行局部设置
        //[Produces("application/json",//当遇到以下 Accept 值时，实际返回 application/json（视频P43）
        //                             //将忽视 Startup.cs 中对输出格式化器的全局设置，导致当前方法不支持 xml
        //                             "application/vnd.company.hateoas+json",
        //                             "application/vnd.company.friendly+json",
        //                             "application/vnd.company.friendly.hateoas+json",
        //                             "application/vnd.company.full+json",
        //                             "application/vnd.company.full.hateoas+json")]
        public async Task<IActionResult> GetCompany(Guid companyId,
                                                    string fields,
                                                    [FromHeader(Name="Accept")]  //mediaType 从 Header 获取
                                                    string acceptMediaType)
        {
            //判断 Uri Query 中的 fields 字符串是否合法（视频P39）
            if (! _propertyCheckerService.TypeHasProperties<CompanyDto>(fields))
            {
                return BadRequest();  //返回状态码400
            }

            //尝试解析 MediaTypeHeaderValue（视频P43）
            if (! MediaTypeHeaderValue.TryParse(acceptMediaType, out MediaTypeHeaderValue parsedAcceptMediaType))
            {
                return BadRequest();  //返回状态码400
            }

            var company = await _companyRepository.GetCompanyAsync(companyId);
            if (company == null)
            {
                return NotFound();  //返回状态码404
            }

            //是否需要 links（HATEOAS）（视频P41-43）
            bool includeLinks = parsedAcceptMediaType
                               .SubTypeWithoutSuffix
                               .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase); //忽略大小写
            //是否需要 Full Dto
            bool isFull = parsedAcceptMediaType
                         .SubTypeWithoutSuffix
                         .ToString()
                         .Contains("full", StringComparison.InvariantCultureIgnoreCase);

            var shapedData = isFull ?
                         _mapper.Map<CompanyFullDto>(company).ShapeData(fields)
                         :
                         _mapper.Map<CompanyDto>(company).ShapeData(fields);

            if (includeLinks)
            {
                var companyDict = shapedData as IDictionary<string, object>;
                var links = CreateLinksForCompany(companyId, fields);
                companyDict.Add("links", links);
                return Ok(companyDict);
            }

            return Ok(shapedData);
        }

        #endregion HttpGet

        #region HttpPost

        [HttpPost(Name = nameof(CreateCompany))]
        [RequestHeaderMatchesMediaType("Content-Type", //当 Content-Type 是以下 value 时，使用该方法（相当于路由）（视频P44）
                                                      "application/json",
                                                      "application/vnd.company.companyforcreation+json")]
        //指明该方法可以消费哪些格式的 Content-Type（视频P44）
        [Consumes("application/json","application/vnd.company.companyforcreation+json")]
        public async Task<IActionResult> CreateCompany([FromBody]CompanyAddDto company,
                                                       [FromHeader(Name="Accept")]
                                                       string acceptMediaType)
        {
            //使用 [ApiController] 属性后，会自动返回400错误，无需再使用以下代码：
            //if (!ModelState.IsValid) { return UnprocessableEntity(ModelState); }

            //Core 3.x 使用 [ApiController] 属性后，无需再使用以下代码：
            //if (company == null) { return BadRequest(); }

            //尝试解析 MediaTypeHeaderValue（视频P43）
            if (! MediaTypeHeaderValue.TryParse(acceptMediaType, out MediaTypeHeaderValue parsedAcceptMediaType))
            {
                return BadRequest();  //返回状态码400
            }

            //是否需要 links（HATEOAS）（视频P41-43）
            bool includeLinks = parsedAcceptMediaType
                               .SubTypeWithoutSuffix
                               .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase); //忽略大小写
            //是否需要 Full Dto
            bool isFull = parsedAcceptMediaType
                         .SubTypeWithoutSuffix
                         .ToString()
                         .Contains("full", StringComparison.InvariantCultureIgnoreCase);

            var entity = _mapper.Map<Company>(company);
            _companyRepository.AddCompany(entity);
            await _companyRepository.SaveAsync();

            var shapedData = isFull ?
                             _mapper.Map<CompanyFullDto>(entity).ShapeData(null)
                             :
                             _mapper.Map<CompanyDto>(entity).ShapeData(null);

            if (includeLinks)
            {
                var companyDict = shapedData as IDictionary<string, object>;
                var links = CreateLinksForCompany(entity.Id, null);
                companyDict.Add("links", links);
                //返回状态码201
                //通过使用 CreatedAtRoute 返回时可以在 Header 中添加一个地址（Loaction）
                return CreatedAtRoute(nameof(GetCompany), new { companyId = entity.Id }, companyDict);
            }

            return CreatedAtRoute(nameof(GetCompany), new { companyId = entity.Id }, shapedData);
        }

        //含 KankruptTime 的 Create Company Api（视频P44）
        //传入 CompanyAddWithBankruptTimeDto
        [HttpPost(Name = nameof(CreateCompanyWithBankruptTime))]
        [RequestHeaderMatchesMediaType("Content-Type", //当 Content-Type 是以下 value 时，使用该方法（相当于路由）（视频P44）
                                                      "application/vnd.company.companyforcreationwithbankrupttime+json")]
        //指明该方法可以消费哪些格式的 Content-Type（视频P44）
        [Consumes("application/vnd.company.companyforcreationwithbankrupttime+json")]
        public async Task<IActionResult> CreateCompanyWithBankruptTime([FromBody]CompanyAddWithBankruptTimeDto company,
                                                                       [FromHeader(Name="Accept")]
                                                                       string acceptMediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(acceptMediaType, out MediaTypeHeaderValue parsedAcceptMediaType))
            {
                return BadRequest();
            }
            bool includeLinks = parsedAcceptMediaType
                               .SubTypeWithoutSuffix
                               .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
            bool isFull = parsedAcceptMediaType
                         .SubTypeWithoutSuffix
                         .ToString()
                         .Contains("full", StringComparison.InvariantCultureIgnoreCase);
            var entity = _mapper.Map<Company>(company);
            _companyRepository.AddCompany(entity);
            await _companyRepository.SaveAsync();
            var shapedData = isFull ?
                             _mapper.Map<CompanyFullDto>(entity).ShapeData(null)
                             :
                             _mapper.Map<CompanyDto>(entity).ShapeData(null);
            if (includeLinks)
            {
                var companyDict = shapedData as IDictionary<string, object>;
                var links = CreateLinksForCompany(entity.Id, null);
                companyDict.Add("links", links);
                return CreatedAtRoute(nameof(GetCompany), new { companyId = entity.Id }, companyDict);
            }
            return CreatedAtRoute(nameof(GetCompany), new { companyId = entity.Id }, shapedData);
        }

        #endregion HttpPost

        #region HttpDelete

        [HttpDelete("{companyId}", Name = nameof(DeleteCompany))]
        public async Task<IActionResult> DeleteCompany(Guid companyId)
        {
            var companyEntity = await _companyRepository.GetCompanyAsync(companyId);
            if (companyEntity == null)
            {
                return NotFound();
            }
            //删除 Company 时将属于它的 Employees 一并删除
            //把 Employees 加载到内存中，使删除时可以追踪 ？？？（视频P33）
            await _companyRepository.GetEmployeesAsync(companyId, new EmployeeDtoParameters());

            _companyRepository.DeleteCompany(companyEntity);
            await _companyRepository.SaveAsync();
            return NoContent();
        }

        #endregion HttpDelete

        #region HttpOptions

        [HttpOptions]
        public IActionResult GetCompaniesOptions()
        {
            Response.Headers.Add("Allow", "DELETE,GET,PATCH,PUT,OPTIONS");
            return Ok();
        }

        #endregion HttpOptions
        #endregion Controllers

        #region Functions

        /// <summary>
        /// 生成上一页、下一页或当前页的 URI（视频P35）
        /// </summary>
        /// <param name="parameters">CompanyDtoParameters</param>
        /// <param name="type">ResourceUriType</param>
        /// <returns>跳转到目标页的 Uri</returns>
        private string CreateCompaniesResourceUri(CompanyDtoParameters parameters,
                                                  ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage: //上一页
                    return Url.Link(
                        nameof(GetCompanies), //方法名
                        new                   //Uri Query 字符串参数
                        {
                            pageNumber = parameters.PageNumber - 1,
                            pageSize = parameters.PageSize,
                            companyName = parameters.companyName,
                            searchTerm = parameters.SearchTerm,
                            orderBy = parameters.OrderBy, //排序（视频P38）
                            fields = parameters.Fields  //数据塑形（视频P39）
                        }); ;

                case ResourceUriType.NextPage: //下一页
                    return Url.Link(
                        nameof(GetCompanies),
                        new
                        {
                            pageNumber = parameters.PageNumber + 1,
                            pageSize = parameters.PageSize,
                            companyName = parameters.companyName,
                            searchTerm = parameters.SearchTerm,
                            orderBy = parameters.OrderBy,
                            fields = parameters.Fields
                        });

                //case ResourceUriType.CurrentPage: //当前页
                default:
                    return Url.Link(
                        nameof(GetCompanies),
                        new
                        {
                            pageNumber = parameters.PageNumber,
                            pageSize = parameters.PageSize,
                            companyName = parameters.companyName,
                            searchTerm = parameters.SearchTerm,
                            orderBy = parameters.OrderBy,
                            fields = parameters.Fields
                        });
            }
        }

        /// <summary>
        /// 为 Company 单个资源创建 HATEOAS 的 links（视频P41）
        /// </summary>
        /// <param name="companyId">Company Id</param>
        /// <param name="fields">fields 字符串</param>
        /// <returns>Company 单个资源的 links</returns>
        private IEnumerable<LinkDto> CreateLinksForCompany(Guid companyId, string fields)
        {
            var links = new List<LinkDto>();

            //GetCompany 的 link
            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(new LinkDto(Url.Link(nameof(GetCompany), new { companyId }), //href - 超链接
                                      "self",                                          //rel - 与当前资源的关系或描述
                                      "GET"));                                         //method - 方法
            }
            else
            {
                links.Add(new LinkDto(Url.Link(nameof(GetCompany), new { companyId, fields }),
                                      "self",
                                      "GET"));
            }

            //DeleteCompany 的 link
            links.Add(new LinkDto(Url.Link(nameof(DeleteCompany), new { companyId, fields }),
                                      "delete_company",
                                      "DELETE"));

            //CreateEmployeeForCompany 的 link
            links.Add(new LinkDto(Url.Link(nameof(EmployeesController.CreateEmployeeForCompany), new { companyId }),
                                      "create_employee_for_company",
                                      "POST"));

            //GetEmployeesForCompany 的 link
            links.Add(new LinkDto(Url.Link(nameof(EmployeesController.GetEmployeesForCompany), new { companyId }),
                                      "employees",
                                      "GET"));

            return links;
        }

        /// <summary>
        /// 为 Companies 集合资源创建 HATEOAS 的 links（视频P42）
        /// </summary>
        /// <param name="parameters">CompanyDtoParameters</param>
        /// <param name="hasPrevious">是否有上一页</param>
        /// <param name="hasNext">是否有下一页</param>
        /// <returns>GetCompanies 集合资源的 links</returns>
        private IEnumerable<LinkDto> CreateLinksForCompany(CompanyDtoParameters parameters, bool hasPrevious, bool hasNext)
        {
            var links = new List<LinkDto>();

            //CurrentPage 当前页链接
            links.Add(new LinkDto(CreateCompaniesResourceUri(parameters, ResourceUriType.CurrentPage),
                      "self",
                      "GET"));

            if (hasPrevious)
            {
                //PreviousPage 上一页链接
                links.Add(new LinkDto(CreateCompaniesResourceUri(parameters, ResourceUriType.PreviousPage),
                          "previous_page",
                          "GET"));
            }

            if (hasNext)
            {
                //NextPage 下一页链接
                links.Add(new LinkDto(CreateCompaniesResourceUri(parameters, ResourceUriType.NextPage),
                          "next_page",
                          "GET"));
            }

            return links;
        }

        #endregion Functions
    }
}