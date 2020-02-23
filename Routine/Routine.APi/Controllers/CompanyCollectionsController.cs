using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Routine.APi.Entities;
using Routine.APi.Helpers;
using Routine.APi.Models;
using Routine.APi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Routine.APi.Controllers
{
    [ApiController]
    [Route("api/companycollections")]
    public class CompanyCollectionsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICompanyRepository _companyRepository;

        public CompanyCollectionsController(IMapper mapper, ICompanyRepository companyRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _companyRepository = companyRepository ??
                                    throw new ArgumentNullException(nameof(companyRepository));
        }

        #region Controllers
        #region HttpGet

        //自定义 Model 绑定器，获取集合资源（视频P24）
        [HttpGet("({ids})", Name = nameof(GetCompanyCollection))]
        public async Task<IActionResult> GetCompanyCollection([FromRoute]
                                                              [ModelBinder(BinderType = typeof(ArrayModelBinder))]
                                                              IEnumerable<Guid> ids)
        {
            if (ids == null)
            {
                return BadRequest();
            }
            var entities = await _companyRepository.GetCompaniesAsync(ids);

            if (ids.Count() != entities.Count())
            {
                return NotFound();
            }

            //使用 Company Full Dto
            var dtosToReturn = _mapper.Map<IEnumerable<CompanyFullDto>>(entities);
            return Ok(dtosToReturn);
        }

        #endregion HttpGet

        #region HttpPost

        //同时创建父子关系的资源（视频P23）
        [HttpPost]
        public async Task<IActionResult> CreateCompanyCollection(IEnumerable<CompanyAddDto> companyCollection) //Task<IActionResult> = Task<ActionResult<IEnumerable<CompanyDto>>>
        {
            //.Net Core 会自动对所有 Company 与 Employee 资源进行模型验证

            var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);
            foreach (var company in companyEntities)
            {
                _companyRepository.AddCompany(company);
            }
            await _companyRepository.SaveAsync();
            var dtosToReturn = _mapper.Map<IEnumerable<CompanyFriendlyDto>>(companyEntities);
            var idsString = string.Join(",", dtosToReturn.Select(x => x.Id));
            return CreatedAtRoute(nameof(GetCompanyCollection), new { ids = idsString }, dtosToReturn);
        }

        #endregion HttpPost
        #endregion Controllers
    }
}
