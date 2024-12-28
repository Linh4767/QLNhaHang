using Microsoft.AspNetCore.Mvc;

namespace QLNhaHang.Controllers
{
    public class AdminController : Controller
    {
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
