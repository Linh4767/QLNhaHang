using Microsoft.AspNetCore.Components.Forms;
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

        //Quản lý bàn ăn
        public IActionResult DSBanAn(string? searchQuery)
        {
            var dsBan = string.IsNullOrEmpty(searchQuery) 
                        ? _QLNhaHangContext.Bans.ToList() 
                        : _QLNhaHangContext.Bans.Where(e => e.ViTri.Contains(searchQuery)).ToList();
            ViewData["SearchQuery"] = searchQuery;
            return View(dsBan);
        }

        //thêm bàn mới
        public IActionResult ThemBan()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ThemBan(Ban ban)
        {
            //lấy mã bàn lớn nhất hiện tại
            var maCuoi = _QLNhaHangContext.Bans
                .OrderByDescending(b => b.MaBan)
                .FirstOrDefault();

            //tạo mã bàn mới
            if (maCuoi != null)
            {
                //lấy phần số từ mã bàn cuối cùng và tăng lên
                var soCuoi = int.Parse(maCuoi.MaBan.Substring(1));
                ban.MaBan = "B" + (soCuoi + 1).ToString("D3");
            }
            else
            {
                //trường hợp chưa có bàn nào trong database
                ban.MaBan = "B001";
            }

            //mặc định trạng thái bàn là trống
            ban.TrangThai = false;

            //kiểm tra ký tự đặc biệt
            var regex = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9\s]+$");
            //kiểm tra số lượng ký tự của trường vị trí
            if (ban.ViTri.Length > 30)
            {
                ModelState.AddModelError("ViTri", "Vị trí không được vượt quá 30 ký tự.");
            }
            else if (!regex.IsMatch(ban.ViTri))
            {
                ModelState.AddModelError("ViTri", "Vị trí chỉ được chứa chữ, số và khoảng trắng.");
            }

            //ktra số lượng
            if (!ban.SoLuongNguoi.HasValue || ban.SoLuongNguoi.Value <= 0 || ban.SoLuongNguoi.Value > 20)
            {
                ModelState.AddModelError("SoLuongNguoi", "Số lượng người phải lớn hơn 0 và nhỏ hơn hoặc bằng 20.");
            }

            //kiểm tra trạng thái của ModelState
            if (!ModelState.IsValid)
            {
                return View(ban);
            }

            //thêm bàn mới vào database
            _QLNhaHangContext.Bans.Add(ban);
            _QLNhaHangContext.SaveChanges();

            //thông báo khi thêm thành công
            TempData["ThemBan"] = "Thêm bàn thành công!";
            return RedirectToAction("DSBanAn");
        }

        //sửa vị trí, số lượng người
        public IActionResult CapNhatBan(string maBan)
        {
            var suaBan = _QLNhaHangContext.Bans.FirstOrDefault(e => e.MaBan == maBan);
            return View(suaBan);
        }

        [HttpPost]
        public IActionResult CapNhatBan(Ban suaBan)
        {
            // Tìm bàn theo mã
            var ban = _QLNhaHangContext.Bans.SingleOrDefault(b => b.MaBan == suaBan.MaBan);

            //ko tìm thấy
            if (ban == null)
            {
                return NotFound();
            }

            //kiểm tra ký tự đặc biệt
            var regex = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9\s]+$");         
            //kiểm tra số lượng ký tự của trường vị trí
            if (suaBan.ViTri.Length > 30)
            {
                ModelState.AddModelError("ViTri", "Vị trí không được vượt quá 30 ký tự.");
            } 
            else if(!regex.IsMatch(suaBan.ViTri))
            {
                ModelState.AddModelError("ViTri", "Vị trí chỉ được chứa chữ, số và khoảng trắng.");
            }

            //ktra số lượng
            if (!suaBan.SoLuongNguoi.HasValue || suaBan.SoLuongNguoi.Value <= 0 || suaBan.SoLuongNguoi.Value > 20)
            {
                ModelState.AddModelError("SoLuongNguoi", "Số lượng người phải lớn hơn 0 và nhỏ hơn hoặc bằng 20.");
            }

            //ktra ModelState
            if (!ModelState.IsValid)
            {
                return View(suaBan); // Trả lại View với lỗi
            }

            ////cập nhật dữ liệu
            ban.SoLuongNguoi = suaBan.SoLuongNguoi;
            ban.ViTri = suaBan.ViTri;

            //lưu thay đổi
            _QLNhaHangContext.SaveChanges();

            //thông báo
            TempData["SuaBan"] = "Cập nhật bà thành công";
            return RedirectToAction("DSBanAn");
        }
    }
}
