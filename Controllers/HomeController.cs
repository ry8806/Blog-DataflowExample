using DataFlowExample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace DataFlowExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly BufferBlock<CompetitionEntry> _bufferBlock;
        private readonly ILogger<HomeController> _logger;

        public HomeController(BufferBlock<CompetitionEntry> bufferBlock, ILogger<HomeController> logger)
        {
            _bufferBlock = bufferBlock;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost("enter")]
        public async Task<IActionResult> SubmitEntry([FromBody] CompetitionEntryRequest entry)
        {
            // Sanitise the input
            entry.Email = entry.Email.Trim();

            var newEntry = new CompetitionEntry
            {
                Email = entry.Email,
                Answer = entry.Answer,
                Created = DateTime.UtcNow,
                IPAddress = HttpContext.Connection.RemoteIpAddress ?? HttpContext.Connection.LocalIpAddress
            };

            await _bufferBlock.SendAsync(newEntry);

            return Ok();
        }
    }
}
