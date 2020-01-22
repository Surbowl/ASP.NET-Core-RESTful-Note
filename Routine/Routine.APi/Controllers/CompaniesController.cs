using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Routine.APi.DtoParameters;
using Routine.APi.Entities;
using Routine.APi.Helpers;
using Routine.APi.Models;
using Routine.APi.Services;
using System;
using System.Collections.Generic;
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

        public CompaniesController(ICompanyRepository companyRepository, IMapper mapper)
        {
            _companyRepository = companyRepository ??
                                        throw new ArgumentNullException(nameof(companyRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet(Name = nameof(GetCompanies))]
        [HttpHead] //添加对 Http Head 的支持，使用 Head 请求时不会返回 Body
        public async Task<IActionResult> GetCompanies([FromQuery]CompanyDtoParameters parameters) //Task<IActionResult> = Task<ActionResult<List<CompanyDto>>>
        {
            var companies = await _companyRepository.GetCompaniesAsync(parameters);

            //向 Header 中添加翻页信息（视频P35）
            //上一页的 URI
            var previousPageLink = companies.HasPrevious
                                ? CreateCompaniesResourceUri(parameters, ResourceUnType.PreviousPage) 
                                : null;
            //下一页的 URI
            var nextPageLink = companies.HasNext
                                ? CreateCompaniesResourceUri(parameters, ResourceUnType.NextPage)
                                : null;
            var paginationMetdata = new
            {
                totalCount = companies.TotalCount,
                pageSize = companies.PageSize,
                currentPage = companies.CurrentPage,
                totalPages = companies.TotalPages,
                previousPageLink,
                nextPageLink
            };
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetdata,
                                                                          new JsonSerializerOptions //URI 中的‘&’、‘？’符号不应该被转义，因此改用不安全的 Encoder
                                                                          {
                                                                              Encoder=JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                                                                          }));
            //不使用 AutoMapper
            //var companyDtos = new List<CompanyDto>();
            //foreach(var company in companies)
            //{
            //    companyDtos.Add(new CompanyDto
            //    {
            //        Id = company.Id,
            //        Name = company.Name
            //    });
            //}

            //使用 AutoMapper（视频P12）
            var companyDtos = _mapper.Map<IEnumerable<CompanyDto>>(companies);

            return Ok(companyDtos);  //OK() 返回状态码200
        }

        [HttpGet("{companyId}", Name = nameof(GetCompany))]  //[Route("{companyId}")]
        public async Task<IActionResult> GetCompany(Guid companyId)
        {
            var company = await _companyRepository.GetCompanyAsync(companyId);
            if (company == null)
            {
                return NotFound();  //返回状态码404
            }
            return Ok(_mapper.Map<CompanyDto>(company));
        }

        [HttpPost]
        public async Task<IActionResult> CreateCompany([FromBody]CompanyAddDto company)  //Task<IActionResult> = Task<ActionResult<CompanyDto>
        {
            //使用 [ApiController] 属性后，会自动返回400错误，无需再使用以下代码：
            //if (!ModelState.IsValid)
            //{
            //    return UnprocessableEntity(ModelState);
            //}

            //新版使用 [ApiController] 属性后，无需再手动检查
            //if (company == null)
            //{
            //    return BadRequest(); //返回状态码400
            //}

            var entity = _mapper.Map<Company>(company);
            _companyRepository.AddCompany(entity);
            await _companyRepository.SaveAsync();
            var returnDto = _mapper.Map<CompanyDto>(entity);
            //返回状态码201
            //通过使用 CreatedAtRoute 返回时可以在 Header 中添加一个地址（Loaction）
            return CreatedAtRoute(nameof(GetCompany), new { companyId = returnDto.Id }, returnDto);
        }

        [HttpDelete("{companyId}")]
        public async Task<IActionResult> DeleteEmployeeForCompany(Guid companyId)
        {
            var companyEntity = await _companyRepository.GetCompanyAsync(companyId);
            if (companyEntity == null)
            {
                return NotFound();
            }
            //把 Employees 加载到内存中，使删除时可以追踪 ？？？（视频P33）
            await _companyRepository.GetEmployeesAsync(companyId, null, null);

            _companyRepository.DeleteCompany(companyEntity);
            await _companyRepository.SaveAsync();
            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetCompaniesOptions()
        {
            Response.Headers.Add("Allow", "DELETE,GET,PATCH,PUT,OPTIONS");
            return Ok();
        }

        /// <summary>
        /// 生成上一页或下一页的 URI（视频P35）
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private string CreateCompaniesResourceUri(CompanyDtoParameters parameters,
                                                  ResourceUnType type)
        {
            switch (type)
            {
                case ResourceUnType.PreviousPage: //上一页
                    return Url.Link(
                        nameof(GetCompanies),
                        new
                        {
                            pageNumber = parameters.PageNumber - 1,
                            pageSize = parameters.PageSize,
                            companyName = parameters.companyName,
                            searchTerm = parameters.SearchTerm
                        });

                case ResourceUnType.NextPage: //下一页
                    return Url.Link(
                        nameof(GetCompanies),
                        new
                        {
                            pageNumber = parameters.PageNumber + 1,
                            pageSize = parameters.PageSize,
                            companyName = parameters.companyName,
                            searchTerm = parameters.SearchTerm
                        });

                default: //当前页
                    return Url.Link(
                        nameof(GetCompanies),
                        new
                        {
                            pageNumber = parameters.PageNumber,
                            pageSize = parameters.PageSize,
                            companyName = parameters.companyName,
                            searchTerm = parameters.SearchTerm
                        });
            }
        }
    }
}
