using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace net.manager.Controllers;
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    [HttpGet(Name = "User/{name}")]
    public void Add(string name)
    {
        HttpClientProxy.CLIENT.CallAddAsync(name);
    }

}