using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace UsersMS.Controllers
{
    [ApiController]
    public class UsersController : ControllerBase
    {
        [HttpGet("api/[controller]/[action]")]
        public async Task<IActionResult> All()
        {
            return Ok(UsersMS.HostedServices.UserHostedService.Users);
        }
    }
}