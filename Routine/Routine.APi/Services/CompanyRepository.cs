﻿using Microsoft.EntityFrameworkCore;
using Routine.APi.Data;
using Routine.APi.DtoParameters;
using Routine.APi.Entities;
using Routine.APi.Helpers;
using Routine.APi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Routine.APi.Services
{
    /// <summary>
    /// Company Repository
    /// </summary>
    public class CompanyRepository : ICompanyRepository
    {
        private readonly RoutineDbContext _context;
        private readonly IPropertyMappingService _propertyMappingService;

        public CompanyRepository(RoutineDbContext context,IPropertyMappingService propertyMappingService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _propertyMappingService = propertyMappingService 
                                      ?? throw new ArgumentNullException(nameof(propertyMappingService));
        }

        public void AddCompany(Company company)
        {
            if (company == null)
            {
                throw new ArgumentNullException(nameof(company));
            }
            company.Id = Guid.NewGuid();
            if (company.Employees != null)
            {
                foreach (var employee in company.Employees)
                {
                    employee.Id = Guid.NewGuid();
                }
            }
            _context.Companies.Add(company);
        }

        public void AddEmployee(Guid companyId, Employee employee)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            if (employee == null)
            {
                throw new ArgumentNullException(nameof(employee));
            }

            employee.CompanyId = companyId;
            _context.Employees.Add(employee);
        }

        public async Task<bool> CompanyExistsAsync(Guid companyId)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(companyId));
            }

            return await _context.Companies.AnyAsync(x => x.Id == companyId);
        }

        public void DeleteCompany(Company company)
        {
            if (company == null)
            {
                throw new ArgumentNullException(nameof(company));
            }

            _context.Companies.Remove(company);
        }

        public void DeleteEmployee(Employee employee)
        {
            if (employee == null)
            {
                throw new ArgumentNullException(nameof(employee));
            }

            _context.Employees.Remove(employee);
        }

        public async Task<Company> GetCompanyAsync(Guid companyId)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(companyId));
            }

            return await _context.Companies.FirstOrDefaultAsync(x => x.Id == companyId);
        }

        public async Task<PagedList<Company>> GetCompaniesAsync(CompanyDtoParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var queryExpression = _context.Companies as IQueryable<Company>;
            //查找指定公司
            if (! string.IsNullOrWhiteSpace(parameters.companyName))
            {
                parameters.companyName = parameters.companyName.Trim();
                queryExpression = queryExpression.Where(x => x.Name == parameters.companyName);
            }
            //模糊搜索
            if (! string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                parameters.SearchTerm = parameters.SearchTerm.Trim();
                queryExpression = queryExpression.Where(x => x.Name.Contains(parameters.SearchTerm)
                                                             || x.Introduction.Contains(parameters.SearchTerm));
            }
            //排序（视频P38）
            if (! string.IsNullOrWhiteSpace(parameters.OrderBy))
            {
                //取得属性映射关系字典
                var mappingDictionary = _propertyMappingService.GetPropertyMapping<CompanyFullDto, Company>();
                //ApplySort 是一个自己定义的拓展方法
                //传入 orderBy 字符串与属性映射关系字典
                //返回排序好的 IQueryable 资源集合
                queryExpression = queryExpression.ApplySort(parameters.OrderBy, mappingDictionary);
            }

            //return await queryExpression.Skip((parameters.PageNumber - 1) * parameters.PageSize)
            //                            .Take(parameters.PageSize)
            //                            .ToListAsync();

            //返回经过翻页处理的 PagedList<T>，PagedList<T> 会执行翻页操作，并保存页码等信息
            return await PagedList<Company>.CreateAsync(queryExpression, parameters.PageNumber, parameters.PageSize);
        }

        public async Task<IEnumerable<Company>> GetCompaniesAsync(IEnumerable<Guid> companyIds)
        {
            if (companyIds == null)
            {
                throw new ArgumentNullException(nameof(companyIds));
            }

            return await _context.Companies
                .Where(x => companyIds.Contains(x.Id))
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<Employee> GetEmployeeAsync(Guid companyId, Guid employeeId)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            if (employeeId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(employeeId));
            }

            return await _context.Employees
                .Where(x => x.Id == employeeId && x.CompanyId == companyId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Employee>> GetEmployeesAsync(Guid companyId, EmployeeDtoParameters parameters)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(companyId));
            }

            var queryExpression = _context.Employees.Where(x => x.CompanyId == companyId);

            //性别筛选
            if (! string.IsNullOrWhiteSpace(parameters.GenderDisplay))
            {
                parameters.GenderDisplay = parameters.GenderDisplay.Trim();
                var gender = Enum.Parse<Gender>(parameters.GenderDisplay);
                queryExpression = queryExpression.Where(x => x.Gender == gender);
            }
            //查询
            if (! string.IsNullOrWhiteSpace(parameters.Q))
            {
                parameters.Q = parameters.Q.Trim();
                queryExpression = queryExpression.Where(x => x.EmployeeNo.Contains(parameters.Q)
                                            || x.FirstName.Contains(parameters.Q)
                                            || x.LastName.Contains(parameters.Q));
            }
            //排序（视频P36-P37）
            if (!string.IsNullOrWhiteSpace(parameters.OrderBy))
            {
                //取得映射关系字典
                var mappingDictionary = _propertyMappingService.GetPropertyMapping<EmployeeDto, Employee>();
                //ApplySort 是一个自己定义的拓展方法
                //传入 FormQuery 中的 OrderBy 字符串与映射关系字典
                //返回排序好的字符串
                queryExpression = queryExpression.ApplySort(parameters.OrderBy, mappingDictionary);
            }
            
            return await queryExpression.ToListAsync();
        }

        public void UpdateCompany(Company company)
        {
            //使用 EF，无需显式地声明
            //_context.Entry(company).State = EntityState.Modified;
        }

        public void UpdateEmployee(Employee employee)
        {
            //使用 EF，无需显式地声明
            //_context.Entry(employee).State = EntityState.Modified;
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }
    }
}
