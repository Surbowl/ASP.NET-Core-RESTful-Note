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
        public async Task<IActionResult> GetEmployeesForCompany(Guid companyId)
        {
            if (await _companyRepository.CompanyExistsAsync(companyId))
            {
                var employees = await _companyRepository.GetEmployeesAsync(companyId);
                var employeeDtos = _mapper.Map<IEnumerable<EmployeeDto>>(employees);
                return Ok(employeeDtos);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("{employeeId}")]
        public async Task<IActionResult> GetEmployeesForCompany(Guid companyId,Guid employeeId)
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
    }
}
