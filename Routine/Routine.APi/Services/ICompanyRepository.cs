using Routine.APi.DtoParameters;
using Routine.APi.Entities;
using Routine.APi.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Routine.APi.Services
{
    public interface ICompanyRepository
    {
        Task<PagedList<Company>> GetCompaniesAsync(CompanyDtoParameters parameters);
        Task<Company> GetCompanyAsync(Guid companyId);
        Task<IEnumerable<Company>> GetCompaniesAsync(IEnumerable<Guid> companyIds);
        void AddCompany(Company company);
        void UpdateCompany(Company company);
        void DeleteCompany(Company company);
        Task<bool> CompanyExistsAsync(Guid companyId);

        //在视频P36之前（不使用 DtoParameters，并且没有排序功能）
        Task<IEnumerable<Employee>> GetEmployeesAsync(Guid companyId, string genderDisplay,string q);
        //视频P36之后
        Task<IEnumerable<Employee>> GetEmployeesAsync(Guid companyId, EmployeeDtoParameters parameters);

        Task<Employee> GetEmployeeAsync(Guid companyId, Guid employeeId);
        void AddEmployee(Guid companyId, Employee employee);
        void UpdateEmployee(Employee employee);
        void DeleteEmployee(Employee employee);
        Task<bool> SaveAsync();
    }
}
