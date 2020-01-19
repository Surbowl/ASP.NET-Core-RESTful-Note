using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Routine.APi.Entities;
using Routine.APi.Models;
using Routine.APi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Routine.APi.Controllers
{
    [ApiController]
    [Route("api/companies/{companyId}/employees")]
    public class EmployeesController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;

        public EmployeesController(ICompanyRepository companyRepository, IMapper mapper)
        {
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeesForCompany(Guid companyId,
                                                                [FromQuery(Name ="gender")]string genderDisplay,
                                                                [FromQuery]string q)
        {
            if (await _companyRepository.CompanyExistsAsync(companyId))
            {
                var employees = await _companyRepository.GetEmployeesAsync(companyId,genderDisplay,q);
                var employeeDtos = _mapper.Map<IEnumerable<EmployeeDto>>(employees);
                return Ok(employeeDtos);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("{employeeId}",Name =nameof(GetEmployeeForCompany))]
        public async Task<IActionResult> GetEmployeeForCompany(Guid companyId,Guid employeeId)
        {
            if (await _companyRepository.CompanyExistsAsync(companyId))
            {
                var employee = await _companyRepository.GetEmployeeAsync(companyId,employeeId);
                if (employee == null)
                {
                    return NotFound();
                }
                var employeeDto = _mapper.Map<EmployeeDto>(employee);
                return Ok(employeeDto);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployeeForCompany([FromRoute]Guid companyId,
                                                                  [FromBody]EmployeeAddDto employee)
        //此处的 [FromRoute] 与 [FromBody] 其实不指定也可以，会自动匹配
        {
            if (! await _companyRepository.CompanyExistsAsync(companyId))
            {
                return NotFound();
            }
            var entity = _mapper.Map<Employee>(employee);
            _companyRepository.AddEmployee(companyId, entity);
            await _companyRepository.SaveAsync();
            var returnDto = _mapper.Map<EmployeeDto>(entity);
            return CreatedAtAction(nameof(GetEmployeeForCompany),
                                    new { companyId = returnDto.CompanyId, employeeId = returnDto.Id },
                                    returnDto);
        }

        /// <summary>
        /// 整体更新/替换，PUT不是安全的，但是幂等
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="employeeId"></param>
        /// <param name="employee"></param>
        /// <returns></returns>
        [HttpPut("{employeeId}")]
        public async Task<IActionResult> UpdateEmployeeForCompany(Guid companyId,
                                                                  Guid employeeId,
                                                                  EmployeeUpdateDto employee)
        {
            if(!await _companyRepository.CompanyExistsAsync(companyId))
            {
                return NotFound();
            }

            var employeeEntity = await _companyRepository.GetEmployeeAsync(companyId, employeeId);
            if (employeeEntity == null)
            {
                //不允许客户端生成 Guid
                //return NotFound();

                //允许客户端生成 Guid
                var employeeToAddEntity = _mapper.Map<Employee>(employee);
                employeeToAddEntity.Id = employeeId;
                _companyRepository.AddEmployee(companyId, employeeToAddEntity);
                await _companyRepository.SaveAsync();
                var returnDto = _mapper.Map<EmployeeDto>(employeeToAddEntity);
                return CreatedAtAction(nameof(GetEmployeeForCompany),
                                        new { companyId = companyId, employeeId = employeeId },
                                        returnDto);
            }

            //把 updateDto 映射到 entity
            _mapper.Map(employee, employeeEntity);
            _companyRepository.UpdateEmployee(employeeEntity);
            await _companyRepository.SaveAsync();
            return NoContent(); //返回状态码204
        }
    }
}
