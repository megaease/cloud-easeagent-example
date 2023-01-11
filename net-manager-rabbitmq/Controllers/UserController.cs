using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace net.manager.rabbitmq.Controllers;
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    Send send = new Send();

    [HttpGet(Name = "User/{name}")]
    public void Add(string name)
    {
        send.Publish("add," + name);
    }
    [HttpDelete(Name = "User/{name}")]
    public void Delete(string name)
    {
        send.Publish("delete," + name);
    }

}