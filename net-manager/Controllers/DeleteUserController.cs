using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace net.manager.Controllers;
[ApiController]
[Route("[controller]")]
public class DeleteUserController : ControllerBase
{
    Send send = new Send();
    [HttpGet(Name = "DeleteUser/{name}")]
    public void Delete(string name)
    {
        HttpClientProxy.CLIENT.CallDeleteAsync(name);
    }

}