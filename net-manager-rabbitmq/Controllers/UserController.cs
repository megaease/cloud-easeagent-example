using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace net_manager_rabbitmq.Controllers;
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    [HttpGet(Name = "User/{name}")]
    public void Add(string name)
    {
        Console.WriteLine($"add {name}");
    }
    [HttpDelete(Name = "User/{name}")]
    public void Delete(string name)
    {
        Console.WriteLine($"delete {name}");
    }

}