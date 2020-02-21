using Microsoft.AspNetCore.Mvc;
using Routine.APi.Models;
using System.Collections.Generic;

namespace Routine.APi.Controllers
{
    //根目录（视频P42）
    [Route("api")]
    [ApiController]
    public class RootController : ControllerBase
    {
        [HttpGet(Name =nameof(GetRoot))]
        public IActionResult GetRoot()
        {
            var links = new List<LinkDto>();
            links.Add(new LinkDto(Url.Link(nameof(GetRoot), new { }),
                      "self",
                      "GET"));
            links.Add(new LinkDto(Url.Link(nameof(CompaniesController.GetCompanies), new { }),
                      "companies",
                      "GET"));
            links.Add(new LinkDto(Url.Link(nameof(CompaniesController.CreateCompany), new { }),
                      "create_company",
                      "POST"));
            var resource = new
            {
                links = links
            };
            return Ok(resource);
        }
    }
}
