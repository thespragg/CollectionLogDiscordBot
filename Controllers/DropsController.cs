using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using CollectionLogBot.Helpers;
using CollectionLogBot.Models;
using Microsoft.AspNetCore.Mvc;

namespace CollectionLogBot.Controllers
{
    [Route("api/bot")]
    [ApiController]
    public class DropsController : ControllerBase
    {
        [HttpPost("add")]
        public void AddDrop(GameDrop drop)
        {
            BankHelper.Drops.Add(drop);
        }

        [HttpGet("")]
        public List<string> GetBosses() => CollectionHandler.CollectionNamesByType("boss");
    }
}
