using Microsoft.EntityFrameworkCore;
using Routine.APi.Entities;
using System;

namespace Routine.APi.Data
{
    public class RoutineDbContext : DbContext
    {
        //调用并获取父类的options
        public RoutineDbContext(DbContextOptions<RoutineDbContext> options) : base(options)
        {

        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>().Property(x => x.Name).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Company>().Property(x => x.Introduction).HasMaxLength(500);
            modelBuilder.Entity<Employee>().Property(x => x.EmployeeNo).IsRequired().HasMaxLength(10);
            modelBuilder.Entity<Employee>().Property(x => x.FirstName).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Employee>().Property(x => x.LastName).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Employee>()
                //指明一对多关系（可省略）
                .HasOne(x => x.Company)
                .WithMany(x => x.Employees)
                //外键
                .HasForeignKey(x => x.CompanyId)
                //禁止级联删除：删除 Company 时如果有 Employee，则无法删除
                //.OnDelete(DeleteBehavior.Restrict);
                //允许级联删除：删除 Company 时自动删除拥有的 Employee
                .OnDelete(DeleteBehavior.Cascade);
                //种子数据
            modelBuilder.Entity<Company>().HasData(
                new Company
                {
                    Id = Guid.Parse("bbdee09c-089b-4d30-bece-44df5923716c"),
                    Name = "Microsoft",
                    Introduction = "Great Company"
                },
                new Company
                {
                    Id = Guid.Parse("6fb600c1-9011-4fd7-9234-881379716440"),
                    Name = "Google",
                    Introduction = "Don't be evil"
                },
                new Company
                {
                    Id = Guid.Parse("5efc910b-2f45-43df-afee-620d40542853"),
                    Name = "Alipapa",
                    Introduction = "Fubao Company"
                }
            );
            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id=Guid.Parse("ca268a19-0f39-4d8b-b8d6-5bace54f8027"),
                    CompanyId = Guid.Parse("bbdee09c-089b-4d30-bece-44df5923716c"),
                    DateOfBirth = new DateTime(1955, 10, 28),
                    EmployeeNo = "M001",
                    FirstName = "William",
                    LastName = "Gates",
                    Gender = Gender.男
                },
                new Employee
                {
                    Id = Guid.Parse("265348d2-1276-4ada-ae33-4c1b8348edce"),
                    CompanyId = Guid.Parse("bbdee09c-089b-4d30-bece-44df5923716c"),
                    DateOfBirth = new DateTime(1998, 1, 14),
                    EmployeeNo = "M024",
                    FirstName = "Kent",
                    LastName = "Back",
                    Gender = Gender.男
                },
                new Employee
                {
                    Id = Guid.Parse("47b70abc-98b8-4fdc-b9fa-5dd6716f6e6b"),
                    CompanyId = Guid.Parse("6fb600c1-9011-4fd7-9234-881379716440"),
                    DateOfBirth = new DateTime(1986, 11, 4),
                    EmployeeNo = "G003",
                    FirstName = "Mary",
                    LastName = "King",
                    Gender = Gender.女
                },
                new Employee
                {
                    Id = Guid.Parse("059e2fcb-e5a4-4188-9b46-06184bcb111b"),
                    CompanyId = Guid.Parse("6fb600c1-9011-4fd7-9234-881379716440"),
                    DateOfBirth = new DateTime(1977, 4, 6),
                    EmployeeNo = "G007",
                    FirstName = "Kevin",
                    LastName = "Richardson",
                    Gender = Gender.男
                },
                new Employee
                {
                    Id = Guid.Parse("a868ff18-3398-4598-b420-4878974a517a"),
                    CompanyId = Guid.Parse("5efc910b-2f45-43df-afee-620d40542853"),
                    DateOfBirth = new DateTime(1964, 9, 10),
                    EmployeeNo = "A001",
                    FirstName = "Jack",
                    LastName = "Ma",
                    Gender = Gender.男
                },
                new Employee
                {
                    Id = Guid.Parse("2c3bb40c-5907-4eb7-bb2c-7d62edb430c9"),
                    CompanyId = Guid.Parse("5efc910b-2f45-43df-afee-620d40542853"),
                    DateOfBirth = new DateTime(1997, 2, 6),
                    EmployeeNo = "A201",
                    FirstName = "Lorraine",
                    LastName = "Shaw",
                    Gender = Gender.女
                }
            );
        }
    }
}
