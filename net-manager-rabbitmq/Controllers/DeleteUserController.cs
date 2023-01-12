using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace net.manager.rabbitmq.Controllers;
[ApiController]
[Route("[controller]")]
public class DeleteUserController : ControllerBase
{
    Send send = new Send();
    [HttpGet(Name = "DeleteUser/{name}")]
    public void Delete(string name)
    {
        Console.WriteLine("=================");
        send.Publish("delete," + name);
    }

}