using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreSystem.Models;
using System.Diagnostics;

namespace StoreSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private static bool? _status;

        public HomeController(ILogger<HomeController> logger)
        {
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

        [Route("api/MockStatus")]
        [AllowAnonymous]
        public IActionResult MockStatus([FromQuery] bool? turn)
        {
            DateTime now = DateTime.Now;

            // 設定時間區間 (當天的 10:00 ~ 11:00)
            DateTime start = now.Date.AddHours(11); // 當天 11:00
            DateTime end = now.Date.AddHours(23);   // 當天 23:00
            if (turn != null) _status = turn;
            return Ok(new{status = _status ==null?(now >= start && now <= end):_status });
        }
    }
}
