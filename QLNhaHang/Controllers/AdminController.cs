using Microsoft.AspNetCore.Mvc;
using QLNhaHang.Models;

namespace QLNhaHang.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly QLNhaHangContext _QLNhaHangContext;

        public AdminController(ILogger<AdminController> logger, QLNhaHangContext qLNhaHangContext)
        {
            _logger = logger;
            _QLNhaHangContext = qLNhaHangContext;
        }
        public IActionResult TrangChu_Admin()
        {
            return View();
        }
        public IActionResult DanhSachNhanVien_Admin()
        {
            return View();
        }
        public IActionResult ThemNhanVien()
        {
            return View();
        }
        public IActionResult SuaNhanVien()
        {
            return View();
        }
    }
}
