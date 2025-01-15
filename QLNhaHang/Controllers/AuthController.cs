using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QLNhaHang.Models;
using QLNhaHang.DTOs;
using Microsoft.CodeAnalysis.Scripting;
using System;
using Microsoft.EntityFrameworkCore;

namespace QLNhaHang.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly QLNhaHangContext _QLNhaHangContext;

        public AuthController(QLNhaHangContext qLNhaHangContext)
        {
            _QLNhaHangContext = qLNhaHangContext;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginDTO dto)
        {
            // Tìm tài khoản dựa trên username
            var taiKhoan = await _QLNhaHangContext.TaiKhoans
                .FirstOrDefaultAsync(t => t.TaiKhoan1 == dto.TaiKhoan);

            string viTri = (from tk in _QLNhaHangContext.TaiKhoans
                            join nv in _QLNhaHangContext.NhanViens
                            on tk.MaNv equals nv.MaNv
                            join vt in _QLNhaHangContext.ViTriCongViecs
                            on nv.MaViTriCv equals vt.MaViTriCv
                            where tk.TaiKhoan1 == dto.TaiKhoan
                            select vt.MaViTriCv).FirstOrDefault();

            // Kiểm tra tài khoản tồn tại
            if (taiKhoan == null)
                return Unauthorized("Sai tài khoản hoặc mật khẩu.");

            // Kiểm tra mật khẩu
            if (!BCrypt.Net.BCrypt.Verify(dto.MatKhau, taiKhoan.MatKhau))
                return Unauthorized("Sai tài khoản hoặc mật khẩu.");

            // Đăng nhập thành công
            // Lưu quyền vào session hoặc cookie
            HttpContext.Session.SetString("MaNV", taiKhoan.MaNv);
            HttpContext.Session.SetString("ViTri", viTri);
            // Đăng nhập thành công và chuyển hướng về trang chủ
            return RedirectToAction("TrangChu_Admin", "Admin");
        }
    }
}
