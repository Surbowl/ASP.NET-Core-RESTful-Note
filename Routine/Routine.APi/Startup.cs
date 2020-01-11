using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Routine.APi.Data;
using Routine.APi.Services;

namespace Routine.APi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // 注册服务 This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                /*
                 * 内容协商：
                 * 针对一个响应，当有多种表述格式的时候，选取最佳的一个表述，例如 application/json、application/xml
                 * 
                 * Accept Header 指明服务器输出格式，对应 ASP.NET Core 里的 Output Formatters
                 * 如果服务器不支持客户端请求的媒体类型（Media Type），返回状态码406
                 * 
                 * Content-Type Header 指明服务器输入格式，对应 ASP.NET Core 里的 Input Formatters
                 */

                //启用406状态码
                options.ReturnHttpNotAcceptable = true;

                //OutputFormatters 默认有且只有 Json 格式
                //添加对 XML 格式的支持
                //此时默认格式依然是 Json ,因为 Json 格式位于第一位置
                options.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());

                //如果在 Index 0 位置插入对 XML 格式的支持，那么默认格式是 XML
                //options.OutputFormatters.Insert(0, new XmlDataContractSerializerOutputFormatter());
            });

            //以下是另一种较新的写法，同样实现对 XML 格式的支持
            //services.AddControllers(options =>
            //{
            //    options.ReturnHttpNotAcceptable = true;
            //}).AddXmlDataContractSerializerFormatters();

            //使用 AutoMapper，扫描当前应用域的所有 Assemblies 寻找 AutoMapper 的配置文件
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            //AddScoped 针对每一个 HTTP 请求都会建立一个新的实例
            services.AddScoped<ICompanyRepository, CompanyRepository>();
           
            services.AddDbContext<RoutineDbContext>(options =>
            {
                options.UseSqlServer("Data Source=localhost;DataBase=routine;Integrated Security=SSPI");
            });
        }

        // 路由中间件 This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            /*
             * 添加中间件的顺序非常重要。如果你把授权中间件放在了Controller的后边，
             * 那么即使需要授权，那么请求也会先到达Controller并执行里面的代码，这样的话授权就没有意义了。
             */

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
