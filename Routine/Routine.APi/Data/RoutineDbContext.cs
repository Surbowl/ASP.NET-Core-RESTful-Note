using Microsoft.EntityFrameworkCore;
using Routine.APi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Routine.APi.Data
{
    public class RoutineDbContext : DbContext
    {
        //调用并获取父类的options
        public RoutineDbContext(DbContextOptions<RoutineDbContext>options):base(options)
        {

        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>().Property(x => x.Name).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Company>().Property(x=>x.Introduction).HasMaxLength(500);
            modelBuilder.Entity<Employee>().Property(x=>x.EmployeeNo).IsRequired().HasMaxLength(10);
            modelBuilder.Entity<Employee>().Property(x=>x.FirstName).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Employee>().Property(x=>x.LastName).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Employee>()
                //指明一对多关系（可省略）
                .HasOne(x => x.Company)
                .WithMany(x => x.Employees)
                //外键
                .HasForeignKey(x => x.CompanyId)
                //删除Company时如果有Employee，则无法删除
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Company>().HasData(
                new Company 
                {
                    Id=Guid.Parse("bbdee09c-089b-4d30-bece-44df5923716c"),
                    Name="Microsoft",
                    Introduction="Great Company"
                },
                new Company
                {
                    Id=Guid.Parse("6fb600c1-9011-4fd7-9234-881379716440"),
                    Name="Google",
                    Introduction="Don't be evil"
                },
                new Company
                {
                    Id=Guid.Parse("5efc910b-2f45-43df-afee-620d40542853"),
                    Name="Alipapa",
                    Introduction="Fubao Company"
                }
                );
        }
    }
}
