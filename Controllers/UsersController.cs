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
        [HttpGet("api/[controller]/all")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(UsersMS.HostedServices.PullNewUsersRabbitMqService.Users);
        }
    }
}