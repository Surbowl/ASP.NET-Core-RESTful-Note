using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Routine.APi.DtoParameters;
using Routine.APi.Models;
using Routine.APi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/*
 * HTTP请求：
 * GET      - 查询
 * POST     - 创建/添加
 * PATCH    - 局部修改/更新
 * PUT      - 如果存在就替换，不存在则创建
 * DELETE   - 移除/删除
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
    /*[ApiController]属性并不是强制要求的，但是它会使开发体验更好
     * 它会启用以下行为：
     * 1.要求使用属性路由（Attribute Routing）
     * 2.自动HTTP 400响应
     * 3.推断参数的绑定源
     * 4.Multipart/form-data请求推断
     * 5.错误状态代码的问题详细信息
     */
    [ApiController]
    [Route("api/companies")] //还可用 [Route("api/[controller]")]
    public class CompaniesController:ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;

        public CompaniesController(ICompanyRepository companyRepository,IMapper mapper)
        {
            _companyRepository = companyRepository ??
                                        throw new ArgumentNullException(nameof(companyRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [HttpHead] //添加对 Http Head 的支持，使用 Head 请求时不会返回 Body
        public async Task<IActionResult> GetCompanies([FromQuery]CompanyDtoParameters parameters) //Task<IActionResult> = Task<ActionResult<List<CompanyDto>>>
        {
            var companies = await _companyRepository.GetCompaniesAsync(parameters);

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

            //使用 AutoMapper
            var companyDtos = _mapper.Map<IEnumerable<CompanyDto>>(companies);

            return Ok(companyDtos);  //OK() 返回状态码200
        }

        [HttpGet("{companyId}")]  //还可用 [Route("{companyId}")]
        public async Task<IActionResult> GetCompany(Guid companyId)
        {
            var company = await _companyRepository.GetCompanyAsync(companyId);
            if (company == null)
            {
                return NotFound();  //返回状态码404
            }
            return Ok(_mapper.Map<CompanyDto>(company));
        }

    }
}
