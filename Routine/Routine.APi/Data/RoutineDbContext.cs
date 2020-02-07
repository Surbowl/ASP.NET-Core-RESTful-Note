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
                new Company  //1
                {
                    Id = Guid.Parse("bbdee09c-089b-4d30-bece-44df5923716c"),
                    Name = "Microsoft",
                    Country = "USA",
                    Industry= "Internet",
                    Product ="Software",
                    Introduction = "Great Company"
                },
                new Company  //2
                {
                    Id = Guid.Parse("6fb600c1-9011-4fd7-9234-881379716440"),
                    Name = "Google",
                    Country = "USA",
                    Industry = "Internet",
                    Product = "Software",
                    Introduction = "Don't be evil"
                },
                new Company  //3
                {
                    Id = Guid.Parse("5efc910b-2f45-43df-afee-620d40542853"),
                    Name = "Alipapa",
                    Country = "CN",
                    Industry = "Internet",
                    Product = "Software",
                    Introduction = "Fubao Company"
                },
                new Company  //4
                {
                    Id = Guid.Parse("8cc04f96-2c42-4f76-832e-1903835b0190"),
                    Name = "Huawei",
                    Country = "CN",
                    Industry = "Communication",
                    Product = "Hardware",
                    Introduction = "Building a Smart World of Everything"
                },
                new Company  //5
                {
                    Id = Guid.Parse("d1f1f410-f563-4355-aa91-4774d693363f"),
                    Name = "Xiaomi",
                    Country = "CN",
                    Industry = "Communication",
                    Product = "Hardware",
                    Introduction = "Born for a fever"
                },
                new Company  //6
                {
                    Id = Guid.Parse("19b8d0f9-4fdf-41ab-b172-f2d5d725b6d9"),
                    Name = "Wuliangye",
                    Country = "CN",
                    Industry = "Wine",
                    Product = "Wine",
                    Introduction = "Great Wine"
                },
                new Company  //7
                {
                    Id = Guid.Parse("6c28b511-34f6-43b2-89f6-fa3dab77bcf9"),
                    Name = "UNIQLO",
                    Country = "JP",
                    Industry = "Textile",
                    Product = "Costume",
                    Introduction = "Good clothes"
                },
                new Company  //8
                {
                    Id = Guid.Parse("4ab2b4af-45ce-41b3-8aed-5447c3140330"),
                    Name = "ZARA",
                    Country = "ESP",
                    Industry = "Textile",
                    Product = "Costume",
                    Introduction = "Stylish clothes"
                },
                new Company  //9
                {
                    Id = Guid.Parse("cd11c117-551c-409f-80e9-c15d89fd7ca8"),
                    Name = "Mercedes-Benz",
                    Country = "GER",
                    Industry = "Auto",
                    Product = "Car",
                    Introduction = "The best car"
                },
                new Company  //10
                {
                    Id = Guid.Parse("a39f7877-3849-48a1-b6af-e35b90c73e6a"),
                    Name = "BMW",
                    Country = "GER",
                    Industry = "Auto",
                    Product = "Car",
                    Introduction = "Good car"
                },
                new Company  //11
                {
                    Id = Guid.Parse("eb8fc677-2600-4fdb-a8ef-51c006e7fc20"),
                    Name = "Yahoo!",
                    Country = "USA",
                    Industry = "Internet",
                    Product = "Software",
                    Introduction = "An American web services provider headquartered in Sunnyvale"
                }
            );
            modelBuilder.Entity<Employee>().HasData(
                //Microsoft employees
                new Employee
                {
                    Id = Guid.Parse("ca268a19-0f39-4d8b-b8d6-5bace54f8027"),
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
                    EmployeeNo = "M002",
                    FirstName = "Kent",
                    LastName = "Back",
                    Gender = Gender.男
                },
                //Google employees
                new Employee
                {
                    Id = Guid.Parse("47b70abc-98b8-4fdc-b9fa-5dd6716f6e6b"),
                    CompanyId = Guid.Parse("6fb600c1-9011-4fd7-9234-881379716440"),
                    DateOfBirth = new DateTime(1986, 11, 4),
                    EmployeeNo = "G001",
                    FirstName = "Mary",
                    LastName = "King",
                    Gender = Gender.女
                },
                new Employee
                {
                    Id = Guid.Parse("059e2fcb-e5a4-4188-9b46-06184bcb111b"),
                    CompanyId = Guid.Parse("6fb600c1-9011-4fd7-9234-881379716440"),
                    DateOfBirth = new DateTime(1977, 4, 6),
                    EmployeeNo = "G002",
                    FirstName = "Kevin",
                    LastName = "Richardson",
                    Gender = Gender.男
                },
                new Employee
                {
                    Id = Guid.Parse("910e7452-c05f-4bf1-b084-6367873664a1"),
                    CompanyId = Guid.Parse("6fb600c1-9011-4fd7-9234-881379716440"),
                    DateOfBirth = new DateTime(1982, 3, 1),
                    EmployeeNo = "G003",
                    FirstName = "Frederic",
                    LastName = "Pullan",
                    Gender = Gender.男
                },
                //Alipapa employees
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
                    EmployeeNo = "A002",
                    FirstName = "Lorraine",
                    LastName = "Shaw",
                    Gender = Gender.女
                },
                new Employee
                {
                    Id = Guid.Parse("e32c33a7-df20-4b9a-a540-414192362d52"),
                    CompanyId = Guid.Parse("5efc910b-2f45-43df-afee-620d40542853"),
                    DateOfBirth = new DateTime(2000, 1, 24),
                    EmployeeNo = "A003",
                    FirstName = "Abel",
                    LastName = "Obadiah",
                    Gender = Gender.女
                },
                //Huawei employees
                new Employee
                {
                    Id = Guid.Parse("3fae0ed7-5391-460a-8320-6b0255b62b72"),
                    CompanyId = Guid.Parse("8cc04f96-2c42-4f76-832e-1903835b0190"),
                    DateOfBirth = new DateTime(1972, 1, 12),
                    EmployeeNo = "H001",
                    FirstName = "Alexia",
                    LastName = "More",
                    Gender = Gender.女
                },
                new Employee
                {
                    Id = Guid.Parse("1b863e75-8bd8-4876-8292-e99998bfa4b1"),
                    CompanyId = Guid.Parse("8cc04f96-2c42-4f76-832e-1903835b0190"),
                    DateOfBirth = new DateTime(1999, 12, 6),
                    EmployeeNo = "H002",
                    FirstName = "Barton",
                    LastName = "Robin",
                    Gender = Gender.女
                },
                new Employee
                {
                    Id = Guid.Parse("c8353598-5b34-4529-a02b-dc7e9f93e59b"),
                    CompanyId = Guid.Parse("8cc04f96-2c42-4f76-832e-1903835b0190"),
                    DateOfBirth = new DateTime(1990, 6, 26),
                    EmployeeNo = "H003",
                    FirstName = "Ted",
                    LastName = "Howard",
                    Gender = Gender.男
                },
                new Employee
                {
                    Id = Guid.Parse("ca86eded-a704-4fbc-8d5e-979761a2e0b8"),
                    CompanyId = Guid.Parse("8cc04f96-2c42-4f76-832e-1903835b0190"),
                    DateOfBirth = new DateTime(2000, 2, 2),
                    EmployeeNo = "M003",
                    FirstName = "Victor",
                    LastName = "Burns",
                    Gender = Gender.男
                }
            );
        }
    }
}
