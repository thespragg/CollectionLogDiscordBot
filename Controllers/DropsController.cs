using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CollectionLogBot.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace CollectionLogBot.Controllers
{
    [Route("api/bot")]
    [ApiController]
    public class DropsController : ControllerBase
    {
        [HttpPost("add/{username}/{dropId}")]
        public async Task<string> AddDrop(string username, int dropId)
        {
            await BankHelper.AddItemToBank(dropId, username, DateTime.Now);
            return "Added";
        }

        [HttpGet("")]
        public List<string> GetBosses() => CollectionHandler.CollectionNamesByType("boss");
    }
}
