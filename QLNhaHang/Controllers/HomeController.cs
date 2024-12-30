using Microsoft.AspNetCore.Mvc;
using QLNhaHang.Models;
using System.Diagnostics;

namespace QLNhaHang.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly QLNhaHangContext _QLNhaHangContext;

        public HomeController(ILogger<HomeController> logger, QLNhaHangContext qLNhaHangContext)
        {
            _logger = logger;
            _QLNhaHangContext = qLNhaHangContext;
        }

        public IActionResult TrangChu()
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
    }
}
