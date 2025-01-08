﻿using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLNhaHang.Models;
using System.Net;
using X.PagedList;
using X.PagedList.Extensions;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text;


namespace QLNhaHang.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly QLNhaHangContext _QLNhaHangContext;
        private readonly CloudinarySettings _cloudinarySettings;

        public AdminController(ILogger<AdminController> logger, QLNhaHangContext qLNhaHangContext, IOptions<CloudinarySettings> cloudinarySettings)
        {
            _logger = logger;
            _QLNhaHangContext = qLNhaHangContext;
            _cloudinarySettings = cloudinarySettings.Value;
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

        //Quản Lý Vị Trí Công Việc
        //Hiển thị danh sách vị trí công việc
        public IActionResult ViTriCongViecList(string searchQuery, int? page)
        {
            var vtcv = _QLNhaHangContext.ViTriCongViecs.ToPagedList(page ?? 1, 5);
            return View(vtcv);
        }

        [HttpGet]
        public IActionResult TimKiemViTriCongViec(string searchQuery, int? page)
        {
            int pageSize = 5; // Số lượng kết quả mỗi trang
            int pageNumber = page ?? 1; // Trang hiện tại, mặc định là trang 1

            // Lấy dữ liệu từ database
            var query = _QLNhaHangContext.ViTriCongViecs.ToList();

            // Kiểm tra nếu có từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(searchQuery))
            {
                // Chuyển đổi từ khóa và dữ liệu sang không dấu
                string searchQueryKhongDau = RemoveDiacritics(searchQuery.ToLower());
                query = query
                    .Where(vtcv => RemoveDiacritics(vtcv.TenViTriCv.ToLower()).Contains(searchQueryKhongDau))
                    .ToList();
            }

            // Phân trang và sắp xếp
            var dsTimKiem = query
                .OrderBy(vtcv => vtcv.MaViTriCv)
                .ToPagedList(pageNumber, pageSize);

            ViewBag.SearchQuery = searchQuery; // Lưu từ khóa tìm kiếm vào ViewBag (nếu có)

            return PartialView("_ViTriCongViecContainer", dsTimKiem); // Trả về PartialView
        }


        //Thêm vị trí công việc
        public string VietHoa(string s)
        {
            if (String.IsNullOrEmpty(s))
                return s;

            string result = "";

            //lấy danh sách các từ  

            string[] words = s.Split(' ');

            foreach (string word in words)
            {
                // từ nào là các khoảng trắng thừa thì bỏ  
                if (word.Trim() != "")
                {
                    if (word.Length > 1)
                        result += word.Substring(0, 1).ToUpper() + word.Substring(1).ToLower() + " ";
                    else
                        result += word.ToUpper() + " ";
                }

            }
            return result.Trim();
        }
        /*
         * Quản lý loại món ăn
         */
        //Danh sách loại món ăn
        public IActionResult DanhSachLoaiMonAn_Admin()
        {
            var dsLoaiMA = _QLNhaHangContext.LoaiMonAns.ToList();
            return View(dsLoaiMA);
        }
        //Thêm loại món ăn
        //Tạo mã tự động
        public string TaoMaLoaiMATuDong()
        {
            //Lấy danh sách loại món ăn
            var dsLoaiMA = _QLNhaHangContext.LoaiMonAns.ToList();
            //Tìm mã loại món ăn lớn
            int maLoaiMALonNhat = dsLoaiMA
                                 .Select(loaiMA => int.Parse(loaiMA.MaLoaiMa.Substring(3)))
                                 .Max();  // Lấy số lớn nhất
            //Tăng chức vụ lớn nhất lên 1
            int maLoaiMAHT = maLoaiMALonNhat + 1;
            return "LMA" + maLoaiMAHT.ToString("D3");
        }
        //Thêm loại món ăn
        public IActionResult ThemLoaiMA()
        {
            var loaiMA = new LoaiMonAn
            {
                MaLoaiMa = TaoMaLoaiMATuDong()
            };
            return View(loaiMA);
        }
        [HttpPost]
        public IActionResult ThemLoaiMA(LoaiMonAn loaiMA)
        {
            loaiMA.MaLoaiMa = TaoMaLoaiMATuDong();
            //ModelState.Remove("MaLoaiMa"); // Xóa lỗi nếu có cho thuộc tính này.
            if (!string.IsNullOrEmpty(loaiMA.TenLoaiMa) || !string.IsNullOrWhiteSpace(loaiMA.TenLoaiMa))
            {

                var regex = new System.Text.RegularExpressions.Regex(@"^(?!.*\s{2})[\p{L}\s]+$");
                if (!regex.IsMatch(loaiMA.TenLoaiMa))
                {
                    ModelState.AddModelError("TenLoaiMa", "Tên danh mục chỉ được chứa chữ cái, khoảng trắng và không được có 2 khoảng trắng liên tiếp.");
                }
                if (loaiMA.TenLoaiMa.Length > 60)
                {
                    ModelState.AddModelError("TenLoaiMa", "Tên danh mục không vượt quá 60 ký tự");
                    //TempData["ThongBaoVuotQuaGH"] = "Tên danh mục không vượt quá 60 ký tự";
                    //return RedirectToAction("SuaLoaiMA");
                }
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).ToList();
                    foreach (var error in errors)
                    {
                        Console.WriteLine("MaLoaiMa: " + loaiMA.MaLoaiMa);
                        // Log hoặc kiểm tra chi tiết lỗi
                        Console.WriteLine(error.ErrorMessage);

                    }
                    return View(loaiMA); // Trả lại View với lỗi
                }

                //if (loaiMA.TenLoaiMa.Length > 60)
                //{
                //    TempData["ThongBaoVuotQuaGH"] = "Tên danh mục không vượt quá 60 ký tự";
                //    return RedirectToAction("ThemLoaiMA");
                //}
                else
                {

                    if (!_QLNhaHangContext.LoaiMonAns.Any(loaiMonAn => loaiMonAn.TenLoaiMa.ToLower() == loaiMA.TenLoaiMa.ToLower()))
                    {
                        loaiMA.TenLoaiMa = VietHoa(loaiMA.TenLoaiMa);
                        _QLNhaHangContext.LoaiMonAns.Add(loaiMA);
                        _QLNhaHangContext.SaveChanges();
                        TempData["ThongBaoThemTC"] = "Thêm danh mục món ăn thành công";
                        return RedirectToAction("DanhSachLoaiMonAn_Admin");
                    }
                    else
                    {
                        TempData["ThongBaoThemLoi"] = "Danh mục món ăn đã tồn tại";
                        return View("ThemLoaiMA");
                    }
                }
            }
            else
            {
                TempData["ThongBaoTrong"] = "Vui lòng không để trống tên loại món ăn";
                return View("ThemLoaiMA");
            }

        }
        //Xóa loại món ăn
        public IActionResult XoaLoaiMA(string maLoaiMA)
        {
            // Kiểm tra nếu có món ăn đang bán
            var dsMonAn = _QLNhaHangContext.MonAns.Where(ma => ma.LoaiMa == maLoaiMA && (ma.TrangThai == 1 || ma.TrangThai == 0)).ToList();
            if (dsMonAn.Any())
            {
                TempData["ThongBaoXoaLoi"] = "Danh sách món ăn trong mục vẫn đang được bán. Danh mục không thể xóa.";
                return RedirectToAction("DanhSachLoaiMonAn_Admin");
            }

            // Nếu không có món ăn đang bán, tiếp tục xử lý các món ăn ngừng bán
            var dsMonAnNgungBan = _QLNhaHangContext.MonAns.Where(ma => ma.LoaiMa == maLoaiMA && ma.TrangThai == 2).ToList();
            if (dsMonAnNgungBan.Any())
            {
                foreach (var i in dsMonAnNgungBan)
                {
                    i.LoaiMa = null; // Cập nhật lại LoaiMa của món ăn đang ngừng bán
                }
                _QLNhaHangContext.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
            }

            // Xóa danh mục món ăn
            var danhMuc = _QLNhaHangContext.LoaiMonAns.FirstOrDefault(s => s.MaLoaiMa == maLoaiMA);
            if (danhMuc != null)
            {
                _QLNhaHangContext.LoaiMonAns.Remove(danhMuc);
                _QLNhaHangContext.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
                TempData["ThongBaoXoaTC"] = "Xóa danh mục món ăn thành công."; // Thông báo thành công
            }

            return RedirectToAction("DanhSachLoaiMonAn_Admin");
        }
        //Sửa loại món ăn
        public IActionResult SuaLoaiMA(string maLoaiMA)
        {
            var loaiMA = _QLNhaHangContext.LoaiMonAns.Where(lma => lma.MaLoaiMa == maLoaiMA).FirstOrDefault();
            return View(loaiMA);
        }
        [HttpPost]
        public IActionResult SuaLoaiMA(LoaiMonAn loaiMA)
        {
            if (!string.IsNullOrEmpty(loaiMA.TenLoaiMa) || !string.IsNullOrWhiteSpace(loaiMA.TenLoaiMa))
            {
                var regex = new System.Text.RegularExpressions.Regex(@"^(?!.*\s{2})[\p{L}\s]+$");
                if (!regex.IsMatch(loaiMA.TenLoaiMa))
                {
                    ModelState.AddModelError("TenLoaiMa", "Tên danh mục chỉ được chứa chữ cái, khoảng trắng và không được có 2 khoảng trắng liên tiếp.");
                }
                if (loaiMA.TenLoaiMa.Length > 60)
                {
                    ModelState.AddModelError("TenLoaiMa", "Tên danh mục không vượt quá 60 ký tự");
                }
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).ToList();
                    foreach (var error in errors)
                    {
                        Console.WriteLine("MaLoaiMa: " + loaiMA.MaLoaiMa);
                        // Log hoặc kiểm tra chi tiết lỗi
                        Console.WriteLine(error.ErrorMessage);

                    }
                    return View(loaiMA); // Trả lại View với lỗi
                }

                else
                {

                    if (!_QLNhaHangContext.LoaiMonAns.Any(loaiMonAn => loaiMonAn.MaLoaiMa != loaiMA.MaLoaiMa && loaiMonAn.TenLoaiMa.ToLower() == loaiMA.TenLoaiMa.ToLower()))
                    {
                        loaiMA.TenLoaiMa = VietHoa(loaiMA.TenLoaiMa);
                        _QLNhaHangContext.LoaiMonAns.Update(loaiMA);
                        _QLNhaHangContext.SaveChanges();
                        TempData["ThongBaoSuaTC"] = "Cập nhật danh mục món ăn thành công";
                        return RedirectToAction("DanhSachLoaiMonAn_Admin");
                    }
                    else
                    {
                        TempData["ThongBaoSuaLoi"] = "Danh mục món ăn đã tồn tại";
                        return View("SuaLoaiMA");
                    }

                }
            }
            else
            {
                TempData["ThongBaoTrong"] = "Vui lòng không để trống tên loại món ăn";
                return View("SuaLoaiMA");
            }
        }
        public static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        //Tìm kiếm tên danh mục
        [HttpGet]
        public IActionResult TimKiemLoaiMonAn(string tuKhoa)
        {
            // Lấy toàn bộ dữ liệu từ cơ sở dữ liệu
            var dsLoaiMonAn = _QLNhaHangContext.LoaiMonAns.ToList();

            // Nếu có từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(tuKhoa))
            {
                // Chuyển đổi từ khóa sang không dấu
                string tuKhoaKhongDau = RemoveDiacritics(tuKhoa.ToLower());

                // Lọc dữ liệu trên bộ nhớ
                dsLoaiMonAn = dsLoaiMonAn
                    .Where(lma => RemoveDiacritics(lma.TenLoaiMa.ToLower()).Contains(tuKhoaKhongDau))
                    .ToList();
            }

            // Trả về kết quả
            return PartialView("_LoaiMATableContainer", dsLoaiMonAn);
        }

        public IActionResult ThemViTriCV()
        {
            var danhSachMaVTCV = _QLNhaHangContext.ViTriCongViecs
                           .Select(p => p.MaViTriCv)
                           .ToList();

            // Tìm mã vị trí lớn nhất
            int maVTLonNhat = danhSachMaVTCV
                                 .Select(maCV => int.Parse(maCV.Substring(2)))
                                 .DefaultIfEmpty(0) // Nếu danh sách rỗng, trả về 0
                                 .Max();

            // Tạo mã vị trí mới
            int maVTHT = maVTLonNhat + 1;
            string maCVMoi = "VT" + maVTHT.ToString("D3");

            // Khởi tạo đối tượng ViTriCongViec và gán mã mới
            var vtcv = new ViTriCongViec
            {
                MaViTriCv = maCVMoi, // Gán mã tự động cho model
                TenViTriCv = ""      // Giá trị mặc định cho tên
            };

            // Gửi model đến View
            return View(vtcv);
        }
        [HttpPost]

        public IActionResult ThemViTriCV(ViTriCongViec vtcv)
        {
            if (!string.IsNullOrEmpty(vtcv.TenViTriCv))
            {
                // Chuẩn hóa tên bằng hàm VietHoa
                vtcv.TenViTriCv = VietHoa(vtcv.TenViTriCv);
            }
            var regex = new System.Text.RegularExpressions.Regex(@"^(?!.*\s{2})[\p{L}\s]+$");
            if (!regex.IsMatch(vtcv.TenViTriCv))
            {
                ModelState.AddModelError("TenViTriCv", "Tên Vị Trí Công Việc chỉ được chứa chữ cái và khoảng trắng. Không được chứa số, ký tự đặc biệt, hai khoảng trắng liên tiếp.");
            }
            bool isDuplicate = _QLNhaHangContext.ViTriCongViecs
                        .Any(p => p.TenViTriCv.ToLower() == vtcv.TenViTriCv.ToLower());
            if (isDuplicate)
            {
                ModelState.AddModelError("TenViTriCv", "Tên Vị Trí Công Việc đã tồn tại.");
            }
            if (ModelState.IsValid)
            {
                // Lưu vào database
                _QLNhaHangContext.ViTriCongViecs.Add(vtcv);
                _QLNhaHangContext.SaveChanges();
                TempData["ThongBaoThem"] = "Thêm vị trí công việc thành công.";
                return RedirectToAction("ViTriCongViecList");
            }
            return View(vtcv);
        }

        //Xóa vị trí công việc 
        public IActionResult XoaViTriCV(string maVTCV)
        {
            var vtcb = _QLNhaHangContext.ViTriCongViecs.FirstOrDefault(d => d.MaViTriCv == maVTCV);

            if (vtcb != null)
            {
                // Kiểm tra nếu có công việc nào đang sử dụng vị trí công việc này
                bool hasRelatedRecords = _QLNhaHangContext.NhanViens.Any(ctq => ctq.MaViTriCv == maVTCV);

                if (hasRelatedRecords)
                {
                    TempData["ThongBaoXoa"] = "Không thể xóa vị trí công việc này vì có các dữ liệu liên quan.";
                    return RedirectToAction("ViTriCongViecList");
                }

                // Xóa vị trí công việc nếu không có bản ghi liên quan
                _QLNhaHangContext.ViTriCongViecs.Remove(vtcb);
                _QLNhaHangContext.SaveChanges();

                TempData["ThongBaoXoa"] = "Xóa thành công";
            }
            else
            {
                TempData["ThongBaoXoa"] = "Không tìm thấy vị trí công việc.";
            }

            return RedirectToAction("ViTriCongViecList");
        }


        //Cập nhật vị trí công việc
        public IActionResult CapNhatViTriCV(string maVTCV)
        {
            // Tìm vị trí công việc theo mã
            var vtcv = _QLNhaHangContext.ViTriCongViecs
                            .FirstOrDefault(p => p.MaViTriCv == maVTCV);

            if (vtcv == null)
            {
                // Nếu không tìm thấy vị trí công việc, trả về lỗi hoặc trang khác
                return NotFound();
            }

            // Gửi model đến View
            return View(vtcv);
        }
        [HttpPost]
        public IActionResult CapNhatViTriCV(ViTriCongViec vtcv)
        {
            if (!string.IsNullOrEmpty(vtcv.TenViTriCv))
            {
                // Chuẩn hóa tên bằng hàm VietHoa
                vtcv.TenViTriCv = VietHoa(vtcv.TenViTriCv);
            }

            // Kiểm tra tên hợp lệ với regex
            var regex = new System.Text.RegularExpressions.Regex(@"^(?!.*\s{2})[\p{L}\s]+$");

            if (!regex.IsMatch(vtcv.TenViTriCv))
            {
                ModelState.AddModelError("TenViTriCv", "Tên Vị Trí Công Việc chỉ được chứa chữ cái và khoảng trắng. Không được chứa số, ký tự đặc biệt, hai khoảng trắng liên tiếp.");
            }

            // Kiểm tra trùng tên
            bool isDuplicate = _QLNhaHangContext.ViTriCongViecs
                        .Any(p => p.TenViTriCv.ToLower() == vtcv.TenViTriCv.ToLower() && p.MaViTriCv != vtcv.MaViTriCv);
            if (isDuplicate)
            {
                ModelState.AddModelError("TenViTriCv", "Tên Vị Trí Công Việc đã tồn tại.");
            }

            // Nếu ModelState hợp lệ, cập nhật và lưu vào cơ sở dữ liệu
            if (ModelState.IsValid)
            {
                // Cập nhật thông tin vị trí công việc
                var existingViTri = _QLNhaHangContext.ViTriCongViecs
                                        .FirstOrDefault(p => p.MaViTriCv == vtcv.MaViTriCv);

                if (existingViTri != null)
                {
                    existingViTri.TenViTriCv = vtcv.TenViTriCv;

                    // Lưu thay đổi vào cơ sở dữ liệu
                    _QLNhaHangContext.SaveChanges();
                    TempData["ThongBaoSua"] = "Cập nhật vị trí công việc thành công!";
                    return RedirectToAction("ViTriCongViecList");
                }
            }

            // Nếu có lỗi hoặc không hợp lệ, hiển thị lại form với các lỗi
            return View(vtcv);
        }


        //Quản lý bàn ăn
        public IActionResult DSBanAn(int? floor = 1)
        {
            ViewData["CurrentFloor"] = floor;

            var today = DateTime.Today;

            var dsBan = _QLNhaHangContext.Bans
                         .Where(b => b.ViTri.Contains($"Lầu {floor}"))
                         .ToList();
            var dsBanWithStatus = dsBan.Select(ban => new Ban
            {
                MaBan = ban.MaBan,
                SoLuongNguoi = ban.SoLuongNguoi,
                ViTri = ban.ViTri,
                TrangThai = _QLNhaHangContext.DatBans.Any(db => db.MaBan == ban.MaBan && db.NgayDatBan.Value.Date == today.Date)
                ? true // occupied
                : false // available
            }).ToList();
            return View(dsBanWithStatus);
        }

        //thêm bàn mới
        [HttpGet]
        public IActionResult ThemBan(int floor)
        {
            ViewData["Floor"] = $"Lầu {floor}";
            return View();
        }

        [HttpPost]
        public IActionResult ThemBan(Ban ban)
        {
            ModelState.Remove("MaBan");
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

            //ktra số lượng
            if (!ban.SoLuongNguoi.HasValue || ban.SoLuongNguoi.Value <= 0 || ban.SoLuongNguoi.Value > 20)
            {
                ModelState.AddModelError("SoLuongNguoi", "Số lượng người phải lớn hơn 0 và nhỏ hơn hoặc bằng 20.");
            }

            //kiểm tra trạng thái của ModelState
            if (!ModelState.IsValid)
            {
                ViewData["Floor"] = ban.ViTri;
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

            //lưu thay đổi
            _QLNhaHangContext.SaveChanges();

            //thông báo
            TempData["SuaBan"] = "Cập nhật bàn thành công";
            return RedirectToAction("DSBanAn");
        }

        //Đặt bàn
        [HttpGet]
        public IActionResult DatBan(string maBan)
        {
            ViewData["MaBan"] = maBan;
            return View();
        }

        [HttpPost]
        public IActionResult DatBan(DatBan datBan)
        {
            ModelState.Remove("MaDatBan");
            ModelState.Remove("MaBanNavigation");
            //lấy ngày hiện tại
            string ngayHienTai = DateTime.Now.ToString("yyyyMMdd");

            //lọc các mã đặt bàn trong ngày hiện tại
            var maCuoi = _QLNhaHangContext.DatBans
                .Where(b => b.MaDatBan.StartsWith("DB" + ngayHienTai))
                .OrderByDescending(b => b.MaDatBan)
                .FirstOrDefault();

            //tạo mã mới
            string maMoi;
            if (maCuoi != null)
            {
                //lấy phần số cuối từ mã cuối cùng và tăng lên
                int soCuoi = int.Parse(maCuoi.MaDatBan.Substring(10));
                maMoi = "DB" + ngayHienTai + (soCuoi + 1).ToString("D3");
            }
            else
            {
                //nếu chưa có mã nào trong ngày, bắt đầu từ 001
                maMoi = "DB" + ngayHienTai + "001";
            }

            //gán mã mới cho đối tượng đặt bàn
            datBan.MaDatBan = maMoi;

            //thay đổi trang thái của bàn
            var ban = _QLNhaHangContext.Bans.FirstOrDefault(e => e.MaBan == datBan.MaBan);
            if (ban != null)
            {
                ban.TrangThai = true;
            }

            //ktra số lượng
            if (!datBan.SoNguoiDi.HasValue || datBan.SoNguoiDi.Value <= 0 || datBan.SoNguoiDi.Value > ban.SoLuongNguoi.Value)
            {
                ModelState.AddModelError("SoNguoiDi", "Số lượng người phải lớn hơn 0 và nhỏ hơn hoặc bằng số lượng người bàn có thể chứa.");
            }

            //kiểm tra tên KH
            var regex = new System.Text.RegularExpressions.Regex(@"^(?!.*\s{2})[\p{L}\s]+$");
            if (!string.IsNullOrEmpty(datBan.TenKh) || !string.IsNullOrWhiteSpace(datBan.TenKh))
            {
                if (!regex.IsMatch(datBan.TenKh))
                {
                    ModelState.AddModelError("TenKh", "Tên khách hàng chỉ được chứa chữ cái, khoảng trắng và không được có 2 khoảng trắng liên tiếp.");
                }
                else if (datBan.TenKh.Length > 50)
                {
                    ModelState.AddModelError("TenKh", "Tên khách hàng không vượt quá 50 ký tự");
                }
            }

            //kiểm tra ngày đặt bàn
            if (!datBan.NgayDatBan.HasValue)
            {
                ModelState.AddModelError("NgayDatBan", "Ngày đặt bàn là bắt buộc.");
            }
            else
            {
                if (!string.IsNullOrEmpty(datBan.TenKh) || !string.IsNullOrWhiteSpace(datBan.TenKh))
                {
                    DateTime ngayHT = DateTime.Now;
                    DateTime ngayDatBan = datBan.NgayDatBan.Value;

                    if (ngayDatBan.Date < ngayHT.Date)
                    {
                        ModelState.AddModelError("NgayDatBan", "Ngày đặt bàn không được nhỏ hơn ngày hiện tại.");
                    }
                    else if (ngayDatBan.Date == ngayHT.Date)
                    {
                        // Tính chênh lệch giờ dưới dạng số phút
                        var diffInMinutes = (ngayHT - ngayDatBan).TotalMinutes;
                        if (diffInMinutes < 120) // Kiểm tra nếu thời gian đặt ít hơn 2 tiếng (120 phút)
                        {
                            ModelState.AddModelError("NgayDatBan", "Nếu đặt trong cùng ngày, thời gian đặt phải cách hiện tại ít nhất 2 tiếng!");
                        }
                    }
                }
                else if (string.IsNullOrEmpty(datBan.TenKh) && string.IsNullOrWhiteSpace(datBan.TenKh))
                {
                    DateTime ngayHT = DateTime.Now;
                    DateTime ngayDatBan = datBan.NgayDatBan.Value;

                    if (ngayDatBan.Date < ngayHT.Date)
                    {
                        ModelState.AddModelError("NgayDatBan", "Ngày đặt bàn không được nhỏ hơn ngày hiện tại.");
                    }
                    else if (ngayDatBan.Date == ngayHT.Date && ngayDatBan.TimeOfDay > DateTime.Now.TimeOfDay)
                    {
                        ModelState.AddModelError("NgayDatBan", "Nếu ngày đặt bàn là hôm nay vui lòng ko chỉnh giờ lớn hơn giờ hiện tại");
                    }
                }
            }

            //kiểm tra trạng thái của ModelState
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).ToList();
                foreach (var error in errors)
                {
                    // Log hoặc kiểm tra chi tiết lỗi
                    Console.WriteLine(error.ErrorMessage);

                }
                ViewData["MaBan"] = datBan.MaBan;
                return View(datBan);
            }

            //thêm đặt bàn mới vào database
            _QLNhaHangContext.DatBans.Add(datBan);
            _QLNhaHangContext.SaveChanges();

            //thông báo khi thêm thành công
            TempData["DatBan"] = "Đặt bàn thành công!";
            return RedirectToAction("DSBanAn");
        /*
         * Quản lý món ăn
         */
        //Danh sách món ăn
        public IActionResult DanhSachMonAn_Admin()
        {
            var dsMA = _QLNhaHangContext.MonAns.Include(lma => lma.LoaiMaNavigation);
            TempData.Remove("HinhAnh");
            return View(dsMA);
        }
        //Tạo mã tự động
        //Tạo mã tự động
        public string TaoMaMATuDong()
        {
            //Lấy danh sách loại món ăn
            var dsMA = _QLNhaHangContext.MonAns.ToList();
            //Tìm mã loại món ăn lớn
            int maMALonNhat = dsMA
                                 .Select(loaiMA => int.Parse(loaiMA.MaMonAn.Substring(3)))
                                 .Max();  // Lấy số lớn nhất
            //Tăng chức vụ lớn nhất lên 1
            int maMAHT = maMALonNhat + 1;
            return "MA" + maMAHT.ToString("D3");
        }
        //Thêm món ăn
        public IActionResult ThemMonAn()
        {
            var danhMucMAList = _QLNhaHangContext.LoaiMonAns.ToList();
            ViewBag.danhMucMAList = new SelectList(danhMucMAList, "MaLoaiMa", "TenLoaiMa");

            return View();
        }
        [HttpPost]
        public JsonResult KiemTraTenMonAn(string tenMonAn)
        {
            bool isExist = _QLNhaHangContext.MonAns.Any(ma => ma.TenMonAn.ToLower() == tenMonAn.ToLower());

            // Nếu trùng tên, lưu thông báo vào TempData
            if (isExist)
            {
                TempData["ThongBaoThemLoi"] = "Tên món ăn đã tồn tại.";
            }

            return Json(new { isExist, errorMessage = TempData["ThongBaoThemLoi"] });
        }
        [HttpPost]
        public async Task<IActionResult> ThemMonAn(MonAn monAn, IFormFile HinhAnh)
        {
            ModelState.Remove("MaMonAn");
            monAn.MaMonAn = TaoMaMATuDong();

            // Lấy đuôi file ảnh từ tên file được upload
            var fileExtension = Path.GetExtension(HinhAnh.FileName).ToLower();

            // Đổi tên ảnh theo tên món ăn (loại bỏ đuôi file gốc nếu có)
            var fileNameWithoutExtension = monAn.TenMonAn.Replace(" ", "-").ToLower(); // Tên món ăn, không có đuôi
            var fileName = fileNameWithoutExtension + fileExtension;  // Thêm đuôi file vào cuối

            // Tạo Cloudinary account từ thông tin cấu hình
            var account = new Account(
                _cloudinarySettings.CloudName,
                _cloudinarySettings.ApiKey,
                _cloudinarySettings.ApiSecret
            );
            var cloudinary = new Cloudinary(account);

            // Tạo stream cho file
            var fileStream = HinhAnh.OpenReadStream();
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(fileName, fileStream),
                Folder = "QLNhaHang",
                PublicId = fileNameWithoutExtension
            };

            // Thực hiện upload lên Cloudinary
            var uploadResult = await cloudinary.UploadAsync(uploadParams);

            // Kiểm tra kết quả upload
            if (uploadResult.StatusCode == HttpStatusCode.OK)
            {
                // Nếu upload thành công, lưu URL vào đối tượng MonAn
                monAn.HinhAnh = uploadResult.SecureUrl.ToString();
            }

            // Lưu monAn vào cơ sở dữ liệu (nếu cần)
            monAn.TenMonAn = VietHoa(monAn.TenMonAn);
            _QLNhaHangContext.MonAns.Add(monAn);
            await _QLNhaHangContext.SaveChangesAsync();

            // Trả về View
            return RedirectToAction("DanhSachMonAn_Admin");
        }
    }
}

