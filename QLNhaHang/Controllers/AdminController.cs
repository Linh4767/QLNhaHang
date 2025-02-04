using CloudinaryDotNet.Actions;
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
using System.Text.RegularExpressions;
using Newtonsoft.Json;


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
        private bool IsBanOccupied(string maBan, DateTime selectedDate)
        {
            // Lấy danh sách tất cả các lần đặt bàn trong ngày
            var datBanList = _QLNhaHangContext.DatBans
                              .Where(db => db.MaBan == maBan && db.NgayDatBan.Value.Date == selectedDate.Date)
                              .ToList();

            // Kiểm tra trạng thái của từng lần đặt bàn
            foreach (var datBan in datBanList)
            {
                var hoaDon = _QLNhaHangContext.HoaDons
                                .FirstOrDefault(hd => hd.MaDatBan == datBan.MaDatBan);

                // Nếu chưa có hóa đơn, bàn coi như bị chiếm dụng
                if (hoaDon == null)
                {
                    return true; // occupied
                }

                // Nếu hóa đơn có trạng thái "Chưa thanh toán", bàn cũng coi như bị chiếm dụng
                if (hoaDon.TrangThai == "Chưa thanh toán")
                {
                    return true; // occupied
                }
            }

            // Nếu tất cả các lần đặt bàn đều có hóa đơn "Đã thanh toán", bàn là available
            return false; // available
        }
        public IActionResult DSBanAn(string tenKH, string sdt, DateTime ngayDB, int soNguoiDi, int? floor = 1, string date = null)
        {
            ViewBag.TenKH = tenKH ?? string.Empty;
            ViewBag.Sdt = sdt ?? string.Empty;
            ViewBag.NgayDatBan = ngayDB;
            ViewBag.SoNguoiDi = soNguoiDi;
            ViewData["CurrentFloor"] = floor;

            // Nếu không có ngày được chọn, sử dụng ngày hiện tại
            var selectedDate = string.IsNullOrEmpty(date) ? DateTime.Today : DateTime.Parse(date);
            ViewData["SelectedDate"] = selectedDate.ToString("MM/dd/yyyy");

            // Lọc danh sách bàn theo tầng và trạng thái theo ngày
            var dsBan = _QLNhaHangContext.Bans
                         .Where(b => b.ViTri.Contains($"Lầu {floor}"))
                         .ToList();

            var dsBanWithStatus = dsBan.Select(ban => new Ban
            {
                MaBan = ban.MaBan,
                SoLuongNguoi = ban.SoLuongNguoi,
                ViTri = ban.ViTri,
                TrangThai = IsBanOccupied(ban.MaBan, selectedDate)
            }).ToList();

            // Kiểm tra xem yêu cầu là một AJAX request hay không
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                // Trả về một partial view nếu là AJAX request
                return PartialView("_DSBanAnPartial", dsBanWithStatus);
            }

            // Trả về view bình thường nếu không phải AJAX request
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

        /// <summary>
        /// CRUD Ca Làm.
        /// </summary>
        /// <returns></returns>

        public string TaoMaCaTuDong()
        {
            // Lấy danh sách ca làm hiện có
            var dsCaLam = _QLNhaHangContext.Cas.ToList();

            // Tìm mã ca lớn nhất từ danh sách
            int maCaLonNhat = dsCaLam
                              .Select(maca => int.Parse(maca.MaCa.Substring(3)))
                              .Max();

            // Tạo mã ca mới
            int maCaMoi = maCaLonNhat + 1;

            // Kiểm tra nếu mã ca mới đã tồn tại trong cơ sở dữ liệu
            string maCa = "C" + maCaMoi.ToString("D3");

            // Lặp lại kiểm tra mã ca mới cho đến khi mã ca không bị trùng
            while (_QLNhaHangContext.Cas.Any(c => c.MaCa == maCa))
            {
                maCaMoi++;  // Tăng mã ca
                maCa = "C" + maCaMoi.ToString("D3");  // Tạo lại mã ca mới
            }

            return maCa;
        }

        public IActionResult XemDSThongTinCa()
        {
            var dsCaLam = _QLNhaHangContext.Cas.ToList();
            return View(dsCaLam);
        }

        public IActionResult ThemCaLam()
        {
            var loaiMA = new Ca
            {
                MaCa = TaoMaCaTuDong()
            };
            return View(loaiMA);
        }

        [HttpPost]
        public IActionResult ThemCaLam(Ca calam)
        {
            calam.MaCa = TaoMaCaTuDong(); // Tạo mã ca tự động hoặc giữ nguyên giá trị

            ModelState.Remove("MaCa");

            // Kiểm tra thời gian bắt đầu và kết thúc không được để trống
            if (!calam.ThoiGianBatDau.HasValue || !calam.ThoiGianKetThuc.HasValue)
            {
                ModelState.AddModelError("", "Thời gian bắt đầu và kết thúc không được để trống.");
                return View(calam);
            }

            TimeSpan gioBatDau = calam.ThoiGianBatDau.Value;
            TimeSpan gioKetThuc = calam.ThoiGianKetThuc.Value;

            // Kiểm tra thời gian kết thúc phải lớn hơn thời gian ca làm
            if (gioBatDau >= gioKetThuc)
            {
                ModelState.AddModelError("", "Thời gian kết thúc phải lớn hơn thời gian bắt đầu.");
            }

            // Kiểm tra thời gian hợp lệ
            if (gioBatDau < new TimeSpan(7, 0, 0))
            {
                ModelState.AddModelError("ThoiGianBatDau", "Thời gian bắt đầu phải sau 7:00 sáng.");
            }
            if (gioKetThuc > new TimeSpan(22, 30, 0))
            {
                ModelState.AddModelError("ThoiGianKetThuc", "Thời gian kết thúc không được quá 22:30 tối.");
            }

            TimeSpan thoiGianLamViec = gioKetThuc - gioBatDau;
            double thoiGianLamViecInHours = thoiGianLamViec.TotalHours;

            // Kiểm tra loại ca và thời gian làm việc
            if (calam.LoaiCa == "Part-time" && (thoiGianLamViecInHours < 4 || thoiGianLamViecInHours >= 8))
            {
                ModelState.AddModelError("", "Ca làm part-time phải có thời gian làm việc từ 4 đến dưới 8 tiếng.");
            }
            if (calam.LoaiCa == "Full-time" && (thoiGianLamViecInHours < 8 || thoiGianLamViecInHours > 12))
            {
                ModelState.AddModelError("", "Ca làm full-time phải có thời gian làm việc từ 8 đến dưới hoặc bằng 12 tiếng.");
            }


            // Kiểm tra trùng lặp ca làm
            bool isDuplicate = _QLNhaHangContext.Cas.Any(c =>
                c.ThoiGianBatDau == gioBatDau && c.ThoiGianKetThuc == gioKetThuc
            );

            if (isDuplicate)
            {
                ModelState.AddModelError("", "Ca làm đã tồn tại trong khoảng thời gian này.");
            }

            // Nếu có lỗi, trả về lại view với thông báo lỗi
            if (!ModelState.IsValid)
            {
                return View(calam);
            }

            // Nếu hợp lệ, thêm ca làm vào database
            _QLNhaHangContext.Cas.Add(calam);
            _QLNhaHangContext.SaveChanges();

            return RedirectToAction("XemDSThongTinCa");
        }


        public IActionResult XoaCaLam(string id)
        {
            // Kiểm tra nếu id không hợp lệ
            if (string.IsNullOrEmpty(id))
            {
                ModelState.AddModelError("", "Mã ca không hợp lệ.");
                return RedirectToAction("XemDSThongTinCa");
            }

            // Tìm ca làm với mã ca tương ứng
            var calam = _QLNhaHangContext.Cas.FirstOrDefault(c => c.MaCa == id);

            // Nếu không tìm thấy ca làm, trả về NotFound
            if (calam == null)
            {
                ModelState.AddModelError("", "Ca làm không tồn tại.");
                return RedirectToAction("XemDSThongTinCa");
            }

            try
            {
                // Xóa ca làm
                _QLNhaHangContext.Cas.Remove(calam);
                _QLNhaHangContext.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu

                // Thông báo xóa thành công
                TempData["Message"] = "Ca làm đã được xóa thành công.";
            }
            catch (Exception ex)
            {
                // Nếu gặp lỗi khi xóa, hiển thị thông báo lỗi
                ModelState.AddModelError("", "Có lỗi xảy ra khi xóa ca làm: " + ex.Message);
            }

            // Quay lại trang danh sách ca làm
            return RedirectToAction("XemDSThongTinCa");
        }


        public IActionResult SuaCalam(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound("Không tìm thấy mã ca.");
            }

            // Lấy thông tin ca làm từ cơ sở dữ liệu
            var calam = _QLNhaHangContext.Cas.FirstOrDefault(c => c.MaCa == id);

            if (calam == null)
            {
                return NotFound("Không tìm thấy ca làm với mã được cung cấp.");
            }

            // Trả về view cùng với dữ liệu
            return View(calam);
        }

        [HttpPost]
        public IActionResult SuaCalam(Ca calam)
        {
            if (!calam.ThoiGianBatDau.HasValue || !calam.ThoiGianKetThuc.HasValue)
            {
                ModelState.AddModelError("", "Thời gian bắt đầu và kết thúc không được để trống.");
                return View(calam);
            }

            TimeSpan gioBatDau = calam.ThoiGianBatDau.Value;
            TimeSpan gioKetThuc = calam.ThoiGianKetThuc.Value;

            if (gioBatDau >= gioKetThuc)
            {
                ModelState.AddModelError("", "Thời gian kết thúc phải lớn hơn thời gian bắt đầu.");
            }

            if (gioBatDau < new TimeSpan(7, 0, 0))
            {
                ModelState.AddModelError("ThoiGianBatDau", "Thời gian bắt đầu phải sau 7:00 sáng.");
            }
            if (gioKetThuc > new TimeSpan(22, 30, 0))
            {
                ModelState.AddModelError("ThoiGianKetThuc", "Thời gian kết thúc không được quá 22:30 tối.");
            }

            TimeSpan thoiGianLamViec = gioKetThuc - gioBatDau;
            double thoiGianLamViecInHours = thoiGianLamViec.TotalHours;

            if (calam.LoaiCa == "Part-time" && (thoiGianLamViecInHours < 4 || thoiGianLamViecInHours >= 8))
            {
                ModelState.AddModelError("", "Ca làm part-time phải có thời gian làm việc từ 4 đến dưới 8 tiếng.");
            }
            if (calam.LoaiCa == "Full-time" && (thoiGianLamViecInHours < 8 || thoiGianLamViecInHours > 12))
            {
                ModelState.AddModelError("", "Ca làm full-time phải có thời gian làm việc từ 8 đến dưới hoặc bằng 12 tiếng.");
            }

            bool isDuplicate = _QLNhaHangContext.Cas.Any(c =>
                c.MaCa != calam.MaCa && // Loại trừ ca hiện tại
                c.ThoiGianBatDau == gioBatDau &&
                c.ThoiGianKetThuc == gioKetThuc
            );

            if (isDuplicate)
            {
                ModelState.AddModelError("", "Ca làm đã tồn tại trong khoảng thời gian này.");
            }

            if (!ModelState.IsValid)
            {
                return View(calam);
            }

            var existingCa = _QLNhaHangContext.Cas.FirstOrDefault(c => c.MaCa == calam.MaCa);

            if (existingCa != null)
            {
                existingCa.ThoiGianBatDau = calam.ThoiGianBatDau;
                existingCa.ThoiGianKetThuc = calam.ThoiGianKetThuc;
                existingCa.LoaiCa = calam.LoaiCa;

                _QLNhaHangContext.SaveChanges();
            }

            return RedirectToAction("XemDSThongTinCa");
        }

        [HttpGet]
        public IActionResult DatBan(string tenKH, string sdt, DateTime ngayDB, int soNguoiDi, string maBan, string date = null, int? floor = 1)
        {
            ModelState.Remove("TenKh");
            ModelState.Remove("Sdt");
            ViewBag.TenKH = tenKH;
            ViewBag.Sdt = sdt;
            ViewBag.NgayDatBan = ngayDB;
            ViewBag.SoNguoiDi = soNguoiDi;
            ViewData["CurrentFloor"] = floor;
            if (ViewData["Floor"] != null)
            {
                ViewData["CurrentFloor"] = ViewData["Floor"];
            }
            Console.WriteLine(ViewData["CurrentFloor"]);
            Console.WriteLine(date);
            // Nếu không có ngày được chọn, sử dụng ngày hiện tại
            var selectedDate = string.IsNullOrEmpty(date) ? DateTime.Today : DateTime.Parse(date);
            ViewData["SelectedDate"] = selectedDate.ToString("yyyyy/MM/dd");
            ViewData["MaBan"] = maBan;
            return View();
        }
        public bool KiemTraGioiHanNguoi(string maBan, int soNguoiDi)
        {
            // Lấy thông tin bàn từ database
            var ban = _QLNhaHangContext.Bans.FirstOrDefault(e => e.MaBan == maBan);

            // Nếu bàn không tồn tại, trả về false
            if (ban == null)
            {
                return false;
            }

            // Kiểm tra số lượng người đi có nằm trong giới hạn của bàn
            return soNguoiDi > 0 && soNguoiDi <= ban.SoLuongNguoi;
        }

        [HttpGet]
        public IActionResult ValidateSoNguoiDi(string maBan, int soNguoiDi)
        {
            if (!KiemTraGioiHanNguoi(maBan, soNguoiDi))
            {
                return Json(new
                {
                    isValid = false,
                    errorMessage = "Số lượng người phải lớn hơn 0 và nhỏ hơn hoặc bằng sức chứa của bàn."
                });
            }

            return Json(new
            {
                isValid = true
            });
        }

        [HttpPost]
        public IActionResult DatBan(DatBan datBan)
        {
            ModelState.Remove("MaDatBan");
            ModelState.Remove("TenKh");
            ModelState.Remove("Sdt");
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

            //thêm đặt bàn mới vào database
            _QLNhaHangContext.DatBans.Add(datBan);
            _QLNhaHangContext.SaveChanges();

            //thông báo khi thêm thành công
            TempData["DatBan"] = "Đặt bàn thành công!";
            ViewData["Floor"] = datBan.MaBanNavigation?.ViTri != null
   ? Regex.Match(datBan.MaBanNavigation.ViTri, @"\d+").Value
   : "1";
            Console.WriteLine(ViewData["Floor"]);
            return RedirectToAction("DSBanAn", new { maBan = datBan.MaBan, date = datBan.NgayDatBan.Value.ToString("yyyy-MM-dd"), floor = ViewData["Floor"] });
        }
        /*
         * Quản lý món ăn
         */
        //Danh sách món ăn
        public IActionResult DanhSachMonAn_Admin(int? page)
        {
            var dsMA = _QLNhaHangContext.MonAns.Include(lma => lma.LoaiMaNavigation).ToPagedList(page ?? 1, 5);
            TempData.Remove("HinhAnh");
            return View(dsMA);
        }
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
            var uniquePublicId = fileNameWithoutExtension + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(fileName, fileStream),
                Folder = "QLNhaHang",
                PublicId = uniquePublicId
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

        //Sửa món ăn
        public IActionResult SuaMonAn(string maMonAn)
        {
            Console.WriteLine(maMonAn);
            var danhMucMAList = _QLNhaHangContext.LoaiMonAns.ToList();
            ViewBag.danhMucMAList = new SelectList(danhMucMAList, "MaLoaiMa", "TenLoaiMa");
            var monAn = _QLNhaHangContext.MonAns.Where(ma => ma.MaMonAn == maMonAn).FirstOrDefault();
            ViewBag.CurrentImage = monAn?.HinhAnh ?? string.Empty;
            return View(monAn);
        }
        [HttpPost]
        public JsonResult KiemTraTenMonAnTrung(string tenMonAn, string maMonAn)
        {
            bool isExist = _QLNhaHangContext.MonAns.Any(ma => ma.MaMonAn != maMonAn && ma.TenMonAn.ToLower() == tenMonAn.ToLower());

            // Nếu trùng tên, lưu thông báo vào TempData
            if (isExist)
            {
                TempData["ThongBaoThemLoi"] = "Tên món ăn đã tồn tại.";
            }

            return Json(new { isExist, errorMessage = TempData["ThongBaoThemLoi"] });
        }
        [HttpPost]
        public async Task<IActionResult> SuaMonAn(MonAn monAn, IFormFile HinhAnh)
        {
            ModelState.Remove("MaMonAn");
            ModelState.Remove("HinhAnh");

            // Truy vấn món ăn cũ từ database
            var oldMonAn = await _QLNhaHangContext.MonAns
                .Where(ma => ma.MaMonAn == monAn.MaMonAn)
                .FirstOrDefaultAsync();

            if (oldMonAn == null)
            {
                TempData["Error"] = "Không tìm thấy món ăn!";
                return RedirectToAction("DanhSachMonAn_Admin");
            }

            // Lưu URL ảnh cũ
            var oldImageUrl = oldMonAn.HinhAnh;

            // Kiểm tra nếu có ảnh mới
            // Kiểm tra nếu có ảnh mới
            if (HinhAnh != null)
            {
                // Tạo tên file mới
                var fileExtension = Path.GetExtension(HinhAnh.FileName).ToLower();
                var fileNameWithoutExtension = monAn.TenMonAn.Replace(" ", "-").ToLower(); // Tên món ăn, không có đuôi
                var uniquePublicId = fileNameWithoutExtension + "-" + DateTime.Now.ToString("yyyyMMddHHmmss"); // Tạo publicId duy nhất

                var fileName = uniquePublicId + fileExtension;  // Thêm đuôi file vào cuối

                // Tạo Cloudinary account từ thông tin cấu hình
                var account = new Account(
                    _cloudinarySettings.CloudName,
                    _cloudinarySettings.ApiKey,
                    _cloudinarySettings.ApiSecret
                );
                var cloudinary = new Cloudinary(account);

                // Xóa ảnh cũ trên Cloudinary nếu tồn tại
                if (!string.IsNullOrEmpty(oldImageUrl))
                {
                    var oldImagePublicId = oldImageUrl
                                        .Split(new[] { "/image/upload/" }, StringSplitOptions.None).Last()
                                        .Split(new[] { '/' }, 2).Last().Split('.')[0];


                    // Kiểm tra `oldImagePublicId`
                    Console.WriteLine($"PublicId of old image: {oldImagePublicId}");

                    // Gọi API để xóa ảnh cũ
                    var deleteParams = new DeletionParams(oldImagePublicId);
                    var deletionResult = await cloudinary.DestroyAsync(deleteParams);

                    // Log kết quả trả về từ API
                    Console.WriteLine($"Deletion Result: {deletionResult?.StatusCode}");
                    Console.WriteLine($"Deletion Error (if any): {deletionResult?.Error?.Message}");

                    // Kiểm tra kết quả khi xóa ảnh
                    if (deletionResult.StatusCode != HttpStatusCode.OK)
                    {
                        TempData["DoiTenAnh"] = $"Không thể xóa ảnh cũ trên Cloudinary. Lỗi: {deletionResult?.Error?.Message}";
                        var danhMucMAList = _QLNhaHangContext.LoaiMonAns.ToList();
                        ViewBag.danhMucMAList = new SelectList(danhMucMAList, "MaLoaiMa", "TenLoaiMa");
                        return View(monAn);
                    }
                }

                // Upload ảnh mới lên Cloudinary
                var fileStream = HinhAnh.OpenReadStream();
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(fileName, fileStream),
                    Folder = "QLNhaHang",
                    PublicId = uniquePublicId  // Sử dụng publicId duy nhất
                };

                // Thực hiện upload lên Cloudinary
                var uploadResult = await cloudinary.UploadAsync(uploadParams);

                // Kiểm tra kết quả upload
                if (uploadResult.StatusCode == HttpStatusCode.OK)
                {
                    // Nếu upload thành công, lưu URL vào đối tượng MonAn
                    monAn.HinhAnh = uploadResult.SecureUrl.ToString();
                }
            }
            else
            {
                // Nếu không có ảnh mới, kiểm tra xem tên món ăn có thay đổi không
                if (oldMonAn.TenMonAn != monAn.TenMonAn && !string.IsNullOrEmpty(oldImageUrl))
                {
                    var account = new Account(
                        _cloudinarySettings.CloudName,
                        _cloudinarySettings.ApiKey,
                        _cloudinarySettings.ApiSecret
                    );
                    var cloudinary = new Cloudinary(account);

                    // Trích xuất PublicId từ URL ảnh cũ
                    var oldImagePublicId = oldImageUrl
                                       .Split(new[] { "/image/upload/" }, StringSplitOptions.None).Last()
                                       .Split(new[] { '/' }, 2).Last().Split('.')[0];

                    // Đổi tên ảnh cũ với tên mới (sử dụng tên món ăn mới)
                    var newFileNameWithoutExtension = monAn.TenMonAn.Replace(" ", "-").ToLower();
                    var newUniquePublicId = newFileNameWithoutExtension + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    var renameParams = new RenameParams(oldImagePublicId, $"QLNhaHang/{newUniquePublicId}");

                    var renameResult = await cloudinary.RenameAsync(renameParams);

                    // Kiểm tra kết quả đổi tên
                    if (renameResult.StatusCode != HttpStatusCode.OK)
                    {
                        TempData["XoaLoi"] = $"Không thể đổi tên ảnh trên Cloudinary. Lỗi: {renameResult?.Error?.Message}";
                        var danhMucMAList = _QLNhaHangContext.LoaiMonAns.ToList();
                        ViewBag.danhMucMAList = new SelectList(danhMucMAList, "MaLoaiMa", "TenLoaiMa");
                        return View(monAn);
                    }

                    // Cập nhật URL ảnh mới cho đối tượng `oldMonAn`
                    oldMonAn.HinhAnh = renameResult.SecureUrl.ToString();
                    monAn.HinhAnh = oldMonAn.HinhAnh;
                }
                else if (oldMonAn.TenMonAn == monAn.TenMonAn && !string.IsNullOrEmpty(oldImageUrl))
                {
                    // Nếu không đổi tên ảnh, giữ nguyên URL ảnh cũ
                    oldMonAn.HinhAnh = oldImageUrl;
                    monAn.HinhAnh = oldMonAn.HinhAnh;
                }
            }

            // Cập nhật các thuộc tính khác của món ăn
            oldMonAn.TenMonAn = VietHoa(monAn.TenMonAn);
            oldMonAn.Gia = monAn.Gia;

            oldMonAn.LoaiMa = monAn.LoaiMa;

            await _QLNhaHangContext.Entry(oldMonAn)
    .Reference(ma => ma.LoaiMaNavigation)
    .LoadAsync();
            Console.WriteLine($"LoaiMa: {oldMonAn.LoaiMa}");
            Console.WriteLine($"LoaiMaNavigation: {oldMonAn.LoaiMaNavigation?.TenLoaiMa}");

            oldMonAn.MoTa = monAn.MoTa;
            oldMonAn.TrangThai = monAn.TrangThai;
            oldMonAn.HinhAnh = monAn.HinhAnh;


            // Lưu thay đổi vào cơ sở dữ liệu
            await _QLNhaHangContext.SaveChangesAsync();

            // Trả về View
            return RedirectToAction("DanhSachMonAn_Admin");
        }

        //Tìm kiếm món ăn
        [HttpGet]
        public IActionResult TimKiemTenMonAn(string searchQuery, int? page)
        {
            int pageSize = 5; // Số lượng kết quả mỗi trang
            int pageNumber = page ?? 1; // Trang hiện tại, mặc định là trang 1

            // Lấy dữ liệu từ database
            var query = _QLNhaHangContext.MonAns.Include(lma => lma.LoaiMaNavigation).ToList();

            // Kiểm tra nếu có từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(searchQuery))
            {
                // Chuyển đổi từ khóa và dữ liệu sang không dấu
                string searchQueryKhongDau = RemoveDiacritics(searchQuery.ToLower());
                query = query
                    .Where(ma => RemoveDiacritics(ma.TenMonAn.ToLower()).Contains(searchQueryKhongDau))
                    .ToList();
            }

            // Phân trang và sắp xếp
            var dsTimKiem = query
                .ToPagedList(pageNumber, pageSize);

            ViewBag.SearchQuery = searchQuery; // Lưu từ khóa tìm kiếm vào ViewBag (nếu có)

            return PartialView("_MonAnTableContainer", dsTimKiem); // Trả về PartialView
        }

        public IActionResult XemThongTinDatBan(string maBan, string date = null, int? floor = 1)
        {
            ViewData["CurrentFloor"] = floor;
            if (ViewData["Floor"] != null)
            {
                ViewData["CurrentFloor"] = ViewData["Floor"];
            }
            Console.WriteLine(ViewData["CurrentFloor"]);
            Console.WriteLine(date);
            // Nếu không có ngày được chọn, sử dụng ngày hiện tại
            var selectedDate = string.IsNullOrEmpty(date) ? DateTime.Today : DateTime.Parse(date);
            ViewData["SelectedDate"] = selectedDate.ToString("yyyyy/MM/dd");
            var dsDatBan = _QLNhaHangContext.DatBans
                         .Where(b => b.MaBan == maBan && b.NgayDatBan.Value.Date == selectedDate)
                         .ToList();
            var dsDatBanChuaThanhToan = dsDatBan.Where(db =>
            {
                var hoaDon = _QLNhaHangContext.HoaDons
                                .FirstOrDefault(hd => hd.MaDatBan == db.MaDatBan);

                // Giữ lại các lần đặt chưa có hóa đơn hoặc hóa đơn chưa thanh toán
                return hoaDon == null || hoaDon.TrangThai != "Đã thanh toán";
            }).ToList();
            var datBan = dsDatBanChuaThanhToan.FirstOrDefault();
            return View(datBan);
        }

        public IActionResult SuaTTDatBan(string maDatBan, string maBan, string date = null, int? floor = 1)
        {
            ViewData["CurrentFloor"] = floor;
            if (ViewData["Floor"] != null)
            {
                ViewData["CurrentFloor"] = ViewData["Floor"];
            }
            Console.WriteLine(ViewData["CurrentFloor"]);
            Console.WriteLine(date);
            // Nếu không có ngày được chọn, sử dụng ngày hiện tại
            var selectedDate = string.IsNullOrEmpty(date) ? DateTime.Today : DateTime.Parse(date);
            ViewData["SelectedDate"] = selectedDate.ToString("yyyyy/MM/dd");
            ViewData["MaBan"] = maBan;
            var ttDatBan = _QLNhaHangContext.DatBans
                         .Where(b => b.MaDatBan == maDatBan)
                         .SingleOrDefault();
            return View(ttDatBan);
        }



        [HttpPost]
        public IActionResult SuaTTDatBan(DatBan datBan)
        {
            ModelState.Remove("MaDatBan");
            ModelState.Remove("MaBanNavigation");
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
                        if (ngayDatBan <= ngayHT) // Kiểm tra nếu ngày đặt bàn nhỏ hơn ngày hiện tại
                        {
                            ModelState.AddModelError("NgayDatBan", "Thời gian đặt bàn không thể nhỏ hơn thời gian hiện tại!");
                        }
                        else
                        {
                            // Lấy giờ và phút từ ngayDatBan và ngayHT, bỏ phần giây và mili giây
                            var ngayDatBanWithoutSeconds = ngayDatBan.AddSeconds(-ngayDatBan.Second).AddMilliseconds(-ngayDatBan.Millisecond);
                            var ngayHTWithoutSeconds = ngayHT.AddSeconds(-ngayHT.Second).AddMilliseconds(-ngayHT.Millisecond);

                            // Tính khoảng cách thời gian giữa hai đối tượng DateTime
                            // Tính khoảng cách thời gian giữa hai đối tượng DateTime
                            var diffInMinutes = Math.Round((ngayDatBanWithoutSeconds - ngayHTWithoutSeconds).TotalMinutes); // Làm tròn xuống để bỏ phần nhỏ
                            Console.WriteLine($"Khoảng cách thời gian (phút): {diffInMinutes}");
                            if (diffInMinutes < 120) // Nếu khoảng cách nhỏ hơn 120 phút
                            {
                                ModelState.AddModelError("NgayDatBan", "Nếu đặt trong cùng ngày, thời gian đặt phải cách hiện tại ít nhất 2 tiếng!");
                            }
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
            _QLNhaHangContext.DatBans.Update(datBan);
            _QLNhaHangContext.SaveChanges();

            //thông báo khi thêm thành công
            TempData["DatBan"] = "Sửa thông tin thành công!";
            ViewData["Floor"] = datBan.MaBanNavigation?.ViTri != null
    ? Regex.Match(datBan.MaBanNavigation.ViTri, @"\d+").Value
    : "1";
            Console.WriteLine(ViewData["Floor"]);
            Console.WriteLine("DB" + datBan.NgayDatBan.Value);
            return RedirectToAction("XemThongTinDatBan", new { maBan = datBan.MaBan, date = datBan.NgayDatBan.Value.ToString("yyyy-MM-dd"), floor = ViewData["Floor"] });
        }

        [HttpPost]
        public async Task<JsonResult> KiemTraDatBanChua()
        {
            using (var reader = new System.IO.StreamReader(Request.Body))
            {
                var body = await reader.ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<dynamic>(body);

                string tenKH = data.tenKH;
                string sdt = data.sdt;
                int soNguoiDi = data.soNguoiDi;
                string ngayDatBan = data.ngayDatBan;

                Console.WriteLine("Ngày nhận từ client: " + ngayDatBan);  // Kiểm tra giá trị nhận được

                if (string.IsNullOrEmpty(ngayDatBan))
                {
                    return Json(new { success = false, message = "Ngày đặt bàn không hợp lệ: không có giá trị." });
                }

                DateTime parsedDate;
                // Chuyển chuỗi ngày thành DateTime (theo định dạng yyyy-MM-ddTHH:mm)
                if (!DateTime.TryParseExact(ngayDatBan, "yyyy-MM-ddTHH:mm", null, System.Globalization.DateTimeStyles.None, out parsedDate))
                {
                    return Json(new { success = false, message = "Ngày đặt bàn không đúng định dạng." });
                }

                Console.WriteLine($"Ngày đặt bàn đã phân tích: {parsedDate.ToString("yyyy-MM-ddTHH:mm")}");

                // Kiểm tra xem thông tin đặt bàn đã tồn tại hay chưa
                var exists = _QLNhaHangContext.DatBans.Any(db => db.TenKh == tenKH
                                                                && db.Sdt == sdt
                                                                && db.SoNguoiDi == soNguoiDi
                                                                && db.NgayDatBan.HasValue
                                                                && db.NgayDatBan.Value.Year == parsedDate.Year
                                                                && db.NgayDatBan.Value.Month == parsedDate.Month
                                                                && db.NgayDatBan.Value.Day == parsedDate.Day
                                                                && db.NgayDatBan.Value.Hour == parsedDate.Hour
                                                                && db.NgayDatBan.Value.Minute == parsedDate.Minute);

                if (exists)
                {
                    return Json(new { success = true, message = "Thông tin đặt bàn đã tồn tại." });
                }
                else
                {
                    return Json(new { success = false, message = "Thông tin đặt bàn không trùng lặp." });
                }
            }
        }

        //Chuyển bàn
        public IActionResult DSChuyenBan(int floor = 1, string? selectedTable = null, bool isSelectingNewTable = false)
        {
            // Lưu tầng hiện tại
            ViewData["CurrentFloor"] = floor;
            ViewData["SelectedTable"] = selectedTable;
            ViewData["IsSelectingNewTable"] = isSelectingNewTable;

            var today = DateTime.Today;

            // Lấy danh sách tất cả bàn ở tầng hiện tại
            var dsBan = _QLNhaHangContext.Bans
                          .Where(b => b.ViTri.Contains($"Lầu {floor}"))
                          .ToList();

            // Chia danh sách bàn
            //ds bàn có người
            var dsBanOccupied = dsBan
                .Where(ban => _QLNhaHangContext.DatBans
                .Any(db => db.MaBan == ban.MaBan && db.NgayDatBan.Value.Date == today.Date &&
                    _QLNhaHangContext.HoaDons.Any(hd => hd.MaDatBan == db.MaDatBan && hd.TrangThai == "Chưa thanh toán")))
                .ToList();

            //danh sách bàn trống
            var dsBanAvailable = dsBan
            .Where(ban => !_QLNhaHangContext.DatBans.Any(db => db.MaBan == ban.MaBan && db.NgayDatBan.Value.Date == today.Date))
            .ToList();
            if (selectedTable != null)
            {
                var banCu = _QLNhaHangContext.Bans.FirstOrDefault(b => b.MaBan == selectedTable);
                dsBanAvailable = dsBan
                    .Where(ban => !_QLNhaHangContext.DatBans.Any(db => db.MaBan == ban.MaBan && db.NgayDatBan.Value.Date == today.Date) && ban.SoLuongNguoi >= banCu.SoLuongNguoi)
                    .ToList();
            }


            // Kiểm tra nếu danh sách bàn trống rỗng
            ViewData["IsAvailableEmpty"] = !dsBanAvailable.Any();

            ViewData["OccupiedTables"] = dsBanOccupied;
            ViewData["AvailableTables"] = dsBanAvailable;

            return View();
        }

        [HttpGet]
        public IActionResult ChuyenBan(string maBanCu, string maBanMoi)
        {
            ViewData["MaBanCu"] = maBanCu;
            ViewData["MaBanMoi"] = maBanMoi;

            return View();
        }

        [HttpPost]
        public IActionResult ChuyenBan(LichSuChuyenBan lsCB)
        {
            ModelState.Remove("MaChuyenBan");
            ModelState.Remove("MaBanCuNavigation");
            ModelState.Remove("MaBanMoiNavigation");
            ModelState.Remove("MaDatBanNavigation");
            ModelState.Remove("MaNvNavigation");

            //lấy ngày hiện tại
            string ngayHienTai = DateTime.Now.ToString("yyyyMMdd");

            //lọc các mã đặt bàn trong ngày hiện tại
            var maCuoi = _QLNhaHangContext.LichSuChuyenBans
                .Where(b => b.MaChuyenBan.StartsWith("CB" + ngayHienTai))
                .OrderByDescending(b => b.MaChuyenBan)
                .FirstOrDefault();

            //tạo mã mới
            string maMoi;
            if (maCuoi != null)
            {
                //lấy phần số cuối từ mã cuối cùng và tăng lên
                int soCuoi = int.Parse(maCuoi.MaChuyenBan.Substring(10));
                maMoi = "CB" + ngayHienTai + (soCuoi + 1).ToString("D3");
            }
            else
            {
                //nếu chưa có mã nào trong ngày, bắt đầu từ 001
                maMoi = "CB" + ngayHienTai + "001";
            }

            //gán mã mới cho đối tượng chuyển bàn
            lsCB.MaChuyenBan = maMoi;

            string maDatBan = (from db in _QLNhaHangContext.DatBans
                               join hd in _QLNhaHangContext.HoaDons
                               on db.MaDatBan equals hd.MaDatBan
                               where db.MaBan == lsCB.MaBanCu && db.NgayDatBan.Value.Date == DateTime.Today.Date && hd.TrangThai == "Chưa thanh toán"
                               select db.MaDatBan).SingleOrDefault();

            DateTime ngayHT = DateTime.Now;
            //var maDatBan = layMaDatBan(lsCB.MaBanCu, ngayHT);
            //var maDB = _QLNhaHangContext.DatBans.FirstOrDefault(b => b.MaBan == lsCB.MaBanCu && b.NgayDatBan.Value.Date == DateTime.Today.Date);
            var maDB = _QLNhaHangContext.DatBans.FirstOrDefault(b => b.MaDatBan == maDatBan);

            //thay đổi trạng thái bàn
            var banCu = _QLNhaHangContext.Bans.FirstOrDefault(b => b.MaBan == lsCB.MaBanCu);
            var banMoi = _QLNhaHangContext.Bans.FirstOrDefault(b => b.MaBan == lsCB.MaBanMoi);

            if (banCu == null || banMoi == null)
            {
                TempData["BaoLoi"] = "Không tìm thấy bàn cũ hoặc bàn mới";
                return View("ChuyenBan");
            }

            if (maDB != null)
            {
                lsCB.MaDatBan = maDB.MaDatBan;
                maDB.MaBan = banMoi.MaBan;
            }
            else
            {
                TempData["BaoLoi"] = "Không tìm thấy mã đặt bàn";
                return View("ChuyenBan");
            }

            //kiểm tra trường lý do
            var regex = new System.Text.RegularExpressions.Regex(@"^(?!.*\s{2})[\p{L}\s]+$");
            if (!string.IsNullOrEmpty(lsCB.LyDoChuyen) || !string.IsNullOrWhiteSpace(lsCB.LyDoChuyen))
            {
                if (!regex.IsMatch(lsCB.LyDoChuyen))
                {
                    ModelState.AddModelError("LyDoChuyen", "Lý do chuyển bàn chỉ được chứa chữ cái, khoảng trắng và không được có 2 khoảng trắng liên tiếp.");
                }
            }
            else
            {
                ModelState.AddModelError("LyDoChuyen", "Vui lòng nhập lý do chuyển bàn!");
            }

            banCu.TrangThai = false;
            banMoi.TrangThai = true;

            //kiểm tra trạng thái của ModelState
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).ToList();
                foreach (var error in errors)
                {
                    // Log hoặc kiểm tra chi tiết lỗi
                    Console.WriteLine(error.ErrorMessage);

                }

                ViewData["MaBanCu"] = lsCB.MaBanCu;
                ViewData["MaBanMoi"] = lsCB.MaBanMoi;
                return View(lsCB);
            }

            //thêm chuyển bàn mới vào database
            _QLNhaHangContext.LichSuChuyenBans.Add(lsCB);
            _QLNhaHangContext.SaveChanges();

            //thông báo khi thêm thành công
            TempData["ChuyenBan"] = "Chuyển bàn thành công!";
            return RedirectToAction("DSChuyenBan");
        }

        public IActionResult LSChuyenBan(string? date = null)
        {
            // Nếu không có ngày được chọn, sử dụng ngày hiện tại
            var selectedDate = string.IsNullOrEmpty(date) ? DateTime.Today : DateTime.Parse(date);
            ViewData["SelectedDate"] = selectedDate.ToString("MM/dd/yyyy");

            var dsCB = _QLNhaHangContext.LichSuChuyenBans.Include(nv => nv.MaNvNavigation).Where(l => l.ThoiGianChuyen.Value.Date == selectedDate.Date);
            return View(dsCB);
        }
        public IActionResult DangNhap()
        {
            return View();
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Xóa thông tin đăng nhập khỏi Session
            HttpContext.Session.Clear();

            // Chuyển hướng về trang đăng nhập
            return RedirectToAction("DangNhap");
        }

        //Hiển thị danh sách số lượng trong ca 
        public IActionResult SoLuongTrongCaList(string searchQuery, int? page)
        {
            var sltc = _QLNhaHangContext.SoLuongTrongCas.Include(c => c.MaCaNavigation).ToPagedList(page ?? 1, 5);
            return View(sltc);
        }

        [HttpGet]
        public IActionResult TimKiemSoLuongTrongCa(DateTime? searchDate, int? page)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;

            var query = _QLNhaHangContext.SoLuongTrongCas.Include(c => c.MaCaNavigation).AsQueryable();

            if (searchDate.HasValue)
            {
                query = query.Where(vtcv => vtcv.Ngay.Date == searchDate.Value.Date);
            }

            var dsTimKiem = query
                .OrderBy(vtcv => vtcv.MaQuanLy)
                .ToPagedList(pageNumber, pageSize);

            return PartialView("_SoLuongTrongCaContainer", dsTimKiem);
        }

        //Thêm số lượng trong ca 
        public IActionResult ThemSoLuongTrongCa(string ngay = null, string maCa = null)
        {
            string ngayHienTai = DateTime.Now.ToString("yyyyMMdd");

            // Lấy danh sách loại ca
            var loaiCaList = _QLNhaHangContext.Cas.Select(ca => new
            {
                LoaiCa = ca.LoaiCa
            }).Distinct().ToList();

            // Lấy danh sách mã ca
            var maCaList = _QLNhaHangContext.Cas.Where(ca => ca.LoaiCa == "Full-time").Select(ca => new
            {
                MaCa = ca.MaCa,
                ThoiGianHienThi = $"{DateTime.Today.Add((TimeSpan)ca.ThoiGianBatDau):hh:mm tt} - {DateTime.Today.Add((TimeSpan)ca.ThoiGianKetThuc):hh:mm tt}"
            }).ToList();

            if (string.IsNullOrEmpty(maCa))
            {
                maCa = maCaList.FirstOrDefault()?.MaCa ?? "001";
            }

            var soCuoi = maCa.Substring(1);
            var maQLMoi = $"QL{ngayHienTai}{soCuoi}";

            var sltc = new SoLuongTrongCa
            {
                MaQuanLy = maQLMoi,
                Ngay = DateTime.Parse(ngay ?? DateTime.Now.ToString("yyyy-MM-dd")),
                MaCa = maCa
            };

            ViewBag.LoaiCa = loaiCaList;
            ViewBag.MaCa = new SelectList(maCaList, "MaCa", "ThoiGianHienThi");
            return View(sltc);
        }

        [HttpGet]
        public IActionResult LayDanhSachMaCa(string loaiCa)
        {
            if (string.IsNullOrEmpty(loaiCa))
            {
                return BadRequest("Loại ca không hợp lệ.");
            }

            // Lấy danh sách mã ca theo loại ca
            var list = _QLNhaHangContext.Cas
                .Where(ca => ca.LoaiCa == loaiCa)
                .Select(ca => new
                {
                    MaCa = ca.MaCa,
                    ThoiGianHienThi = $"{DateTime.Today.Add((TimeSpan)ca.ThoiGianBatDau):hh:mm tt} - {DateTime.Today.Add((TimeSpan)ca.ThoiGianKetThuc):hh:mm tt}"
                })
                .ToList();

            if (!list.Any())
            {
                return Json(new { error = "Không có dữ liệu mã ca." });
            }

            return Json(list);
        }

        [HttpGet]
        public IActionResult LayMaQuanLyMoi(string ngay, string maCa)
        {
            if (string.IsNullOrEmpty(ngay) || string.IsNullOrEmpty(maCa))
            {
                return BadRequest("Ngày hoặc mã ca không hợp lệ.");
            }

            // Chuyển ngày thành định dạng yyyyMMdd
            string ngayFormat = DateTime.Parse(ngay).ToString("yyyyMMdd");

            // Lấy 3 số cuối của mã ca
            var soCuoi = maCa.Substring(1);

            // Tạo mã quản lý mới
            string maQuanLy = $"QL{ngayFormat}{soCuoi}";

            // Trả về JSON
            return Json(new { maQuanLy });
        }


        [HttpPost]
        public IActionResult ThemSoLuongTrongCa(SoLuongTrongCa sltc)
        {
            ModelState.Remove("MaCaNavigation");
            // Kiểm tra nếu model không hợp lệ
            if (!sltc.SoLuongToiDa.HasValue || sltc.SoLuongToiDa.Value <= 0)
            {
                ModelState.AddModelError("SoLuongToiDa", "Số lượng nhân viên phải lớn hơn 0");
            }
            if (sltc.Ngay.Date < DateTime.Today.AddDays(7).Date || sltc.Ngay.Date <= DateTime.Today.Date)
            {
                ModelState.AddModelError("Ngay", "Ngày thêm phải là từ 7 ngày sau trở nên tính từ ngày hiện tại.");
            }

            var existingMaQuanLy = _QLNhaHangContext.SoLuongTrongCas.FirstOrDefault(s => s.MaQuanLy == sltc.MaQuanLy);

            if (existingMaQuanLy != null)
            {
                ModelState.AddModelError("MaQuanLy", "Mã Quản Lý đã tồn tại.");
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).ToList();
                foreach (var error in errors)
                {
                    // Log hoặc kiểm tra chi tiết lỗi
                    Console.WriteLine(error.ErrorMessage);

                }
                // Lấy danh sách Loại Ca nếu cần
                ViewBag.LoaiCa = _QLNhaHangContext.Cas.Select(ca => new { LoaiCa = ca.LoaiCa }).Distinct().ToList();

                // Lấy danh sách Mã Ca từ bảng Ca và kết hợp thời gian hiển thị
                var maCaList = _QLNhaHangContext.Cas.Where(ca => ca.LoaiCa == "Full-time").Select(ca => new
                {
                    MaCa = ca.MaCa,
                    ThoiGianHienThi = $"{DateTime.Today.Add((TimeSpan)ca.ThoiGianBatDau):hh:mm tt} - {DateTime.Today.Add((TimeSpan)ca.ThoiGianKetThuc):hh:mm tt}"
                }).ToList();

                // Trả lại SelectList cho mã ca
                ViewBag.MaCa = new SelectList(maCaList, "MaCa", "ThoiGianHienThi");

                return View(sltc);  // Trả lại form nếu có lỗi
            }

            // Tiến hành lưu vào cơ sở dữ liệu
            _QLNhaHangContext.SoLuongTrongCas.Add(sltc);
            _QLNhaHangContext.SaveChanges();  // Lưu thay đổi
            TempData["ThongBaoThem"] = "Thêm số lượng trong ca thành công.";
            // Chuyển hướng về danh sách sau khi thêm thành công
            return RedirectToAction("SoLuongTrongCaList");  // Chuyển hướng đến trang danh sách
        }

        //Xóa số lượng trong ca
        public IActionResult XoaSoLuongTrongCa(string maQuanLy)
        {
            // Tìm bản ghi trong bảng SoLuongTrongCas có MaQuanLy trùng
            var vtcb = _QLNhaHangContext.SoLuongTrongCas
                .FirstOrDefault(d => d.MaQuanLy == maQuanLy);

            if (vtcb != null)
            {
                // Kiểm tra trong bảng SoLuongChiTietTrongCas có bất kỳ bản ghi nào có MaQuanLy trùng
                bool hasRelatedRecords = _QLNhaHangContext.SoLuongChiTietTrongCas
                    .Any(ctq => ctq.MaQuanLy == maQuanLy);

                if (hasRelatedRecords)
                {
                    // Nếu có dữ liệu liên quan, không cho phép xóa
                    TempData["ThongBaoXoa"] = "Không thể xóa vì có các dữ liệu liên quan.";
                    return RedirectToAction("SoLuongTrongCaList");
                }

                // Nếu không có dữ liệu liên quan, tiến hành xóa
                _QLNhaHangContext.SoLuongTrongCas.Remove(vtcb);
                _QLNhaHangContext.SaveChanges();

                TempData["ThongBaoXoa"] = "Xóa thành công";
            }
            else
            {
                // Không tìm thấy bản ghi để xóa
                TempData["ThongBaoXoa"] = "Không tìm thấy số lượng trong ca.";
            }

            return RedirectToAction("SoLuongTrongCaList");
        }

        //Cập nhật số lượng trong ca
        public IActionResult CapNhatSoLuongTrongCa(string maQuanLy)
        {
            var vtcv = _QLNhaHangContext.SoLuongTrongCas
                            .FirstOrDefault(p => p.MaQuanLy == maQuanLy);
            var ca = _QLNhaHangContext.Cas.FirstOrDefault(c => c.MaCa == vtcv.MaCa);

            if (ca != null)
            {

                // Tính toán thời gian hiển thị
                var thoiGianLich = $"{DateTime.Today.Add((TimeSpan)ca.ThoiGianBatDau):hh:mm tt} - {DateTime.Today.Add((TimeSpan)ca.ThoiGianKetThuc):hh:mm tt}";
                ViewBag.LoaiCa = ca.LoaiCa;

                // Gán giá trị vào ViewBag
                ViewBag.ThoiGianLich = thoiGianLich;
            }
            else
            {
                ViewBag.ThoiGianLich = "Không tìm thấy thông tin ca";
            }

            if (vtcv == null)
            {
                // Nếu không tìm thấy vị trí công việc, trả về lỗi hoặc trang khác
                return NotFound();
            }

            // Gửi model đến View
            return View(vtcv);
        }

        [HttpPost]
        public IActionResult CapNhatSoLuongTrongCa(SoLuongTrongCa vtcv)
        {
            ModelState.Remove("MaCaNavigation");

            // Nếu ModelState hợp lệ, thực hiện cập nhật
            if (ModelState.IsValid)
            {
                // Lấy thông tin của SoLuongTrongCa dựa trên MaQuanLy
                var existingViTri = _QLNhaHangContext.SoLuongTrongCas
                                        .FirstOrDefault(p => p.MaQuanLy == vtcv.MaQuanLy);

                if (existingViTri != null)
                {
                    // Chỉ cập nhật trường SoLuongToiDa, các trường khác giữ nguyên
                    existingViTri.SoLuongToiDa = vtcv.SoLuongToiDa;

                    // Lưu thay đổi vào cơ sở dữ liệu
                    _QLNhaHangContext.SaveChanges();

                    // Thông báo thành công
                    TempData["ThongBaoSua"] = "Cập nhật số lượng trong ca thành công!";
                    return RedirectToAction("SoLuongTrongCaList");
                }
                else
                {
                    // Nếu không tìm thấy đối tượng trong cơ sở dữ liệu
                    ModelState.AddModelError("", "Không tìm thấy thông tin số lượng ca.");
                }
            }
            // Nếu có lỗi hoặc không hợp lệ, hiển thị lại form với các lỗi
            return View(vtcv);
        }

        public IActionResult LayLoaiCaTheoMaCa(string maCa)
        {
            if (string.IsNullOrEmpty(maCa))
            {
                return Json(new { loaiCa = "" });
            }

            // Tìm loại ca dựa trên mã ca
            var ca = _QLNhaHangContext.Cas
                        .FirstOrDefault(c => c.MaCa == maCa);

            if (ca != null)
            {
                // Trả về loại ca
                return Json(new { loaiCa = ca.LoaiCa });
            }

            return Json(new { loaiCa = "" }); // Trường hợp không tìm thấy loại ca
        }


        //Hiển thị danh sách chi tiết số lượng ca
        public IActionResult ChiTietSoLuongTrongCaList(string maQuanLy, int? page)
        {
            if (string.IsNullOrEmpty(maQuanLy))
            {
                return NotFound("Mã Quản Lý không hợp lệ.");
            }

            int pageSize = 5; // Số lượng bản ghi mỗi trang
            int pageNumber = page ?? 1; // Trang hiện tại, mặc định là trang 1

            // Truy vấn danh sách chi tiết số lượng ca theo MaQuanLy
            var sltc = _QLNhaHangContext.SoLuongChiTietTrongCas
                .Include(c => c.MaQuanLyNavigation) // Load thông tin liên quan nếu cần
                .Include(c => c.MaViTriCvNavigation) // Load thông tin vị trí công việc
                .Where(c => c.MaQuanLy == maQuanLy) // Lọc theo MaQuanLy
                .OrderBy(c => c.MaQuanLyChiTiet) // Sắp xếp theo thứ tự mã quản lý chi tiết
                .ToPagedList(pageNumber, pageSize);

            ViewBag.MaQuanLy = maQuanLy; // Truyền mã quản lý vào View để sử dụng (nếu cần)

            var ca = _QLNhaHangContext.SoLuongTrongCas
                .FirstOrDefault(s => s.MaQuanLy == maQuanLy);

            // Kiểm tra điều kiện Ngày
            bool isDateConditionMet = ca != null && ca.Ngay >= DateTime.Today.AddDays(7);

            // Truyền điều kiện vào View
            ViewBag.IsDateConditionMet = isDateConditionMet;

            return View(sltc);
        }

        [HttpGet]
        public IActionResult TimKiemChiTietSoLuongTrongCa(string searchQuery, int? page, string maQuanLy)
        {
            int pageSize = 5; // Số lượng kết quả mỗi trang
            int pageNumber = page ?? 1; // Trang hiện tại, mặc định là trang 1

            // Truy vấn dữ liệu từ database
            var query = _QLNhaHangContext.SoLuongChiTietTrongCas
                .Include(c => c.MaViTriCvNavigation) // Bao gồm navigation property
                .Where(s => s.MaQuanLy == maQuanLy) // Lọc theo MaQuanLy
                .AsQueryable();

            // Kiểm tra nếu có từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(searchQuery))
            {
                string searchQueryKhongDau = RemoveDiacritics(searchQuery.ToLower());
                query = query.Where(vtcv => RemoveDiacritics(vtcv.MaViTriCvNavigation.TenViTriCv.ToLower()).Contains(searchQueryKhongDau));
            }

            // Phân trang và sắp xếp
            var dsTimKiem = query
                .OrderBy(vtcv => vtcv.MaQuanLyChiTiet) // Sắp xếp theo MaQuanLyChiTiet
                .ToPagedList(pageNumber, pageSize); // Phân trang

            ViewBag.SearchQuery = searchQuery; // Lưu từ khóa tìm kiếm vào ViewBag (nếu có)

            return PartialView("_ChiTietSoLuongTrongCaContainer", dsTimKiem); // Trả về PartialView
        }

        //Thêm chi tiết số lượng trong ca 
        public IActionResult ThemChiTietSoLuongTrongCa(string maQuanLy)
        {
            if (string.IsNullOrEmpty(maQuanLy))
            {
                return BadRequest("Mã quản lý không được để trống."); // Kiểm tra mã quản lý hợp lệ
            }

            var viTriCongViecList = _QLNhaHangContext.ViTriCongViecs.ToList();

            // Bỏ mã đầu tiên
            var viTriCongViecFilteredList = viTriCongViecList.Skip(1).ToList();

            // Tạo SelectList cho ComboBox
            ViewBag.ViTriCongViecSelectedList = new SelectList(viTriCongViecFilteredList, "MaViTriCv", "TenViTriCv");


            // Lấy mã quản lý chi tiết lớn nhất hiện có cho mã quản lý này
            var maxMaQLChiTiet = _QLNhaHangContext.SoLuongChiTietTrongCas
                .Where(ct => ct.MaQuanLy == maQuanLy)
                .OrderByDescending(ct => ct.MaQuanLyChiTiet)
                .Select(ct => ct.MaQuanLyChiTiet)
                .FirstOrDefault();

            // Tạo mã quản lý chi tiết mới
            var soThuTu = 1; // Nếu chưa có mã nào, bắt đầu từ 1
            if (!string.IsNullOrEmpty(maxMaQLChiTiet))
            {
                var soCuoi = maxMaQLChiTiet.Split('.').Last();
                soThuTu = int.Parse(soCuoi) + 1;
            }
            var maQLChiTietMoi = $"{maQuanLy}.{soThuTu}";

            // Tạo một đối tượng mới để thêm chi tiết
            var chiTiet = new SoLuongChiTietTrongCa
            {
                MaQuanLy = maQuanLy,
                MaQuanLyChiTiet = maQLChiTietMoi,
                SoLuong = 1 // Ban đầu để 0, người dùng sẽ nhập vào
            };

            // Gán danh sách vị trí công việc vào ViewBag
            ViewBag.MaQuanLy = maQuanLy;

            return View(chiTiet); // Trả về View để thêm chi tiết
        }

        [HttpPost]
        public IActionResult ThemChiTietSoLuongTrongCa(SoLuongChiTietTrongCa sltc)
        {
            ModelState.Remove("MaViTriCvNavigation");
            ModelState.Remove("MaQuanLyNavigation");
            if (string.IsNullOrEmpty(sltc.MaQuanLy))
            {
                ModelState.AddModelError("MaQuanLy", "Mã quản lý không được để trống.");
            }

            var soLuongToiDa = _QLNhaHangContext.SoLuongTrongCas
                               .Where(s => s.MaQuanLy == sltc.MaQuanLy)
                               .Select(s => s.SoLuongToiDa)
                               .FirstOrDefault();

            if (sltc.SoLuong > soLuongToiDa)
            {
                ModelState.AddModelError("SoLuong", $"Số lượng không được vượt quá số lượng tối đa là {soLuongToiDa}.");
            }

            var tongSoLuongHienTai = _QLNhaHangContext.SoLuongChiTietTrongCas
                                     .Where(ct => ct.MaQuanLy == sltc.MaQuanLy)
                                     .Sum(ct => ct.SoLuong) ?? 0;

            // Kiểm tra nếu tổng vượt quá `SoLuongToiDa`
            if (tongSoLuongHienTai + sltc.SoLuong > soLuongToiDa)
            {
                ModelState.AddModelError("SoLuong", $"Tổng số lượng nhân viên của ca hiện tại là {tongSoLuongHienTai} nếu thêm {sltc.SoLuong} thì sẽ vượt quá số lượng tối đa là {soLuongToiDa} người.");
            }

            if (!sltc.SoLuong.HasValue || sltc.SoLuong.Value <= 0)
            {
                ModelState.AddModelError("SoLuong", "Số lượng nhân viên phải lớn hơn 0.");
            }

            bool viTriDaTonTai = _QLNhaHangContext.SoLuongChiTietTrongCas.Any(ct => ct.MaQuanLy == sltc.MaQuanLy && ct.MaViTriCv == sltc.MaViTriCv);

            if (viTriDaTonTai)
            {
                ModelState.AddModelError("MaViTriCv", "Vị trí công việc này đã tồn tại trong danh sách.");
            }


            if (!ModelState.IsValid)
            {
                // Xử lý khi form không hợp lệ
                var viTriCongViecList = _QLNhaHangContext.ViTriCongViecs.ToList();
                var viTriCongViecFilteredList = viTriCongViecList.Skip(1).ToList();
                ViewBag.ViTriCongViecSelectedList = new SelectList(viTriCongViecFilteredList, "MaViTriCv", "TenViTriCv");
                ViewBag.MaQuanLy = sltc.MaQuanLy;
                if (sltc.MaViTriCvNavigation != null)
                {
                    Console.WriteLine($"VTCV: {sltc.MaViTriCvNavigation.TenViTriCv}");
                }
                else
                {
                    Console.WriteLine("Vị trí công việc không được tìm thấy.");
                }

                return View(sltc);
            }

            // Lưu vào cơ sở dữ liệu


            _QLNhaHangContext.SoLuongChiTietTrongCas.Add(sltc);
            _QLNhaHangContext.SaveChanges();  // Lưu thay đổi

            TempData["ThongBaoThem"] = "Thêm chi tiết số lượng trong ca thành công.";
            return RedirectToAction("ChiTietSoLuongTrongCaList", new { maQuanLy = sltc.MaQuanLy });
        }

        //Xóa số lượng trong ca
        public IActionResult XoaSoLuongChiTietTrongCa(string maQuanLyChiTiet)
        {
            // Tìm bản ghi trong bảng SoLuongChiTietTrongCa có MaQuanLyChiTiet trùng
            var chiTiet = _QLNhaHangContext.SoLuongChiTietTrongCas
                .FirstOrDefault(ct => ct.MaQuanLyChiTiet == maQuanLyChiTiet);

            if (chiTiet != null)
            {
                // Nếu tìm thấy, tiến hành xóa
                _QLNhaHangContext.SoLuongChiTietTrongCas.Remove(chiTiet);
                _QLNhaHangContext.SaveChanges();

                TempData["ThongBaoXoa"] = "Xóa chi tiết số lượng trong ca thành công.";
            }
            else
            {
                // Không tìm thấy bản ghi
                TempData["ThongBaoXoa"] = "Không tìm thấy chi tiết số lượng trong ca để xóa.";
            }

            return RedirectToAction("ChiTietSoLuongTrongCaList", new { maQuanLy = chiTiet?.MaQuanLy });
        }

        //Cập Nhật Chi Tiết Số Lượng Trong Ca
        public IActionResult CapNhatSoLuongChiTietTrongCa(string maQuanLyChiTiet)
        {
            if (string.IsNullOrEmpty(maQuanLyChiTiet))
            {
                return BadRequest("Mã quản lý chi tiết không hợp lệ.");
            }
            var chiTiet = _QLNhaHangContext.SoLuongChiTietTrongCas.Where(c => c.MaQuanLyChiTiet == maQuanLyChiTiet).FirstOrDefault();
            if (chiTiet == null)
            {
                return NotFound("Không tìm thấy chi tiết số lượng trong ca.");
            }
            var viTriCongViecList = _QLNhaHangContext.ViTriCongViecs.ToList();
            var viTriCongViecFilteredList = viTriCongViecList.Skip(1).ToList();
            ViewBag.ViTriCongViecSelectedList = new SelectList(viTriCongViecFilteredList, "MaViTriCv", "TenViTriCv", chiTiet.MaViTriCv);

            ViewBag.MaQuanLy = chiTiet.MaQuanLy;
            return View(chiTiet); // Trả về View để thêm chi tiết
        }
        [HttpPost]
        public IActionResult CapNhatSoLuongChiTietTrongCa(SoLuongChiTietTrongCa sltc)
        {
            // Loại bỏ các thuộc tính không cần thiết khỏi ModelState
            ModelState.Remove("MaViTriCvNavigation");
            ModelState.Remove("MaQuanLyNavigation");

            // Kiểm tra Mã Quản Lý Chi Tiết
            if (string.IsNullOrEmpty(sltc.MaQuanLyChiTiet))
            {
                ModelState.AddModelError("MaQuanLyChiTiet", "Mã quản lý chi tiết không được để trống.");
            }

            // Lấy số lượng tối đa
            var soLuongToiDa = _QLNhaHangContext.SoLuongTrongCas
                .Where(s => s.MaQuanLy == sltc.MaQuanLy)
                .Select(s => s.SoLuongToiDa)
                .FirstOrDefault();

            // Kiểm tra số lượng nhập vào
            if (sltc.SoLuong > soLuongToiDa)
            {
                ModelState.AddModelError("SoLuong", $"Số lượng không được vượt quá số lượng tối đa là {soLuongToiDa}.");
            }

            // Tính tổng số lượng hiện tại
            var tongSoLuongHienTai = _QLNhaHangContext.SoLuongChiTietTrongCas
                .Where(ct => ct.MaQuanLy == sltc.MaQuanLy && ct.MaQuanLyChiTiet != sltc.MaQuanLyChiTiet)
                .Sum(ct => ct.SoLuong) ?? 0;

            // Kiểm tra tổng số lượng
            if (tongSoLuongHienTai + sltc.SoLuong > soLuongToiDa)
            {
                ModelState.AddModelError("SoLuong", $"Tổng số lượng nhân viên hiện tại là {tongSoLuongHienTai}, nếu cập nhật sẽ vượt quá số lượng tối đa {soLuongToiDa}.");
            }

            // Kiểm tra số lượng hợp lệ
            if (!sltc.SoLuong.HasValue || sltc.SoLuong.Value <= 0)
            {
                ModelState.AddModelError("SoLuong", "Số lượng nhân viên phải lớn hơn 0.");
            }

            // Kiểm tra lỗi trong ModelState
            if (!ModelState.IsValid)
            {
                var viTriCongViecList = _QLNhaHangContext.ViTriCongViecs.ToList();
                var viTriCongViecFilteredList = viTriCongViecList.Skip(1).ToList();
                ViewBag.ViTriCongViecSelectedList = new SelectList(viTriCongViecFilteredList, "MaViTriCv", "TenViTriCv", sltc.MaViTriCv);
                ViewBag.MaQuanLy = sltc.MaQuanLy;
                return View(sltc); // Trả về view với thông tin cũ và lỗi hiển thị
            }

            // Tìm chi tiết cần cập nhật
            var chiTiet = _QLNhaHangContext.SoLuongChiTietTrongCas
                .FirstOrDefault(ct => ct.MaQuanLyChiTiet == sltc.MaQuanLyChiTiet);

            // Kiểm tra nếu không tìm thấy chi tiết
            if (chiTiet == null)
            {
                return RedirectToAction("ChiTietSoLuongTrongCaList", new { maQuanLy = sltc.MaQuanLy });
            }

            // Cập nhật thông tin
            chiTiet.SoLuong = sltc.SoLuong;
            chiTiet.MaViTriCv = sltc.MaViTriCv;

            // Lưu thay đổi vào cơ sở dữ liệu
            _QLNhaHangContext.SaveChanges();

            // Thông báo thành công
            TempData["ThongBaoSua"] = "Cập nhật chi tiết số lượng trong ca thành công.";
            return RedirectToAction("ChiTietSoLuongTrongCaList", new { maQuanLy = sltc.MaQuanLy });
        }
        //Chức năng gọi món
        public IActionResult GoiMon(string maBan, string date = null, int? floor = 1)
        {
            ViewData["CurrentFloor"] = floor;
            if (ViewData["Floor"] != null)
            {
                ViewData["CurrentFloor"] = ViewData["Floor"];
            }
            Console.WriteLine(ViewData["CurrentFloor"]);
            Console.WriteLine(date);
            // Nếu không có ngày được chọn, sử dụng ngày hiện tại
            var selectedDate = string.IsNullOrEmpty(date) ? DateTime.Today : DateTime.Parse(date);
            LayDanhSachMaDatBan(maBan, selectedDate);
            var maDatBan = layMaDatBan(maBan, selectedDate);
            ViewData["MaDatBan"] = maDatBan;
            return View();
        }
        //Lấy danh sách tên loại món ăn
        public IActionResult DSTenLoaiMA()
        {
            var dsLoaiMonAn = _QLNhaHangContext.LoaiMonAns.ToList();
            return PartialView("_TabDanhMucMonAn", dsLoaiMonAn);
        }
        public IActionResult GetMonAnByLoai(string maLoaiMon)
        {
            List<MonAn> dsMonAn;

            if (maLoaiMon == "Tất cả")
            {
                // Lấy tất cả món ăn
                dsMonAn = _QLNhaHangContext.MonAns.ToList();
            }
            else
            {
                // Lấy món ăn theo mã loại món
                dsMonAn = _QLNhaHangContext.MonAns.Where(m => m.LoaiMa == maLoaiMon).ToList();
            }
            // Trả về PartialView với dữ liệu món ăn
            return PartialView("_MonAnList", dsMonAn);
        }
        //Lấy mã đặt bàn
        public void LayDanhSachMaDatBan(string maBan, DateTime ngayDB)
        {
            // Lấy ngày không bao gồm giờ để so sánh
            DateTime ngayChiTiet = ngayDB.Date;

            // Lấy danh sách mã đặt bàn dựa trên điều kiện ngày và mã bàn
            var danhSachMaDatBan = (from db in _QLNhaHangContext.DatBans
                                    where db.MaBan == maBan && db.NgayDatBan.Value.Date == ngayChiTiet
                                    select db.MaDatBan).ToList();

            // Lọc danh sách mã đặt bàn chưa có mã hóa đơn
            var maDatBanChuaCoHoaDon = danhSachMaDatBan
                .Where(maDatBan => !_QLNhaHangContext.HoaDons.Any(hd => hd.MaDatBan == maDatBan))
                .ToList();

            // Tạo mã hóa đơn cho các mã đặt bàn chưa có
            foreach (var maDatBan in maDatBanChuaCoHoaDon)
            {
                TaoHoaDonMoi(maDatBan);
            }
        }
        public string TaoMaHoaDonTuDong()
        {
            //lấy ngày hiện tại
            string ngayHienTai = DateTime.Now.ToString("yyyyMMdd");

            //lọc các mã đặt bàn trong ngày hiện tại
            var maCuoi = _QLNhaHangContext.HoaDons
                .Where(b => b.MaHoaDon.StartsWith("HD" + ngayHienTai))
                .OrderByDescending(b => b.MaHoaDon)
                .FirstOrDefault();

            //tạo mã mới
            string maMoi;
            if (maCuoi != null)
            {
                //lấy phần số cuối từ mã cuối cùng và tăng lên
                int soCuoi = int.Parse(maCuoi.MaHoaDon.Substring(10));
                maMoi = "HD" + ngayHienTai + (soCuoi + 1).ToString("D3");
            }
            else
            {
                //nếu chưa có mã nào trong ngày, bắt đầu từ 001
                maMoi = "HD" + ngayHienTai + "001";
            }

            //gán mã mới cho đối tượng đặt bàn
            return maMoi;
        }
        // Hàm tạo hóa đơn mới
        private void TaoHoaDonMoi(string maDatBan)
        {
            var hoaDonMoi = new HoaDon
            {
                MaHoaDon = TaoMaHoaDonTuDong(),
                MaDatBan = maDatBan,
                NgayXuatHd = null,
                TrangThai = "Chưa thanh toán",
                TongTien = 0 // Hoặc giá trị mặc định khác
            };

            _QLNhaHangContext.HoaDons.Add(hoaDonMoi);
            _QLNhaHangContext.SaveChanges();
        }

        public string layMaDatBan(string maBan, DateTime ngayDB)
        {
            DateTime ngayChiTiet = ngayDB.Date;
            string maDatBan = (from db in _QLNhaHangContext.DatBans
                               join hd in _QLNhaHangContext.HoaDons
                               on db.MaDatBan equals hd.MaDatBan
                               where db.MaBan == maBan && db.NgayDatBan.Value.Date == ngayChiTiet && hd.TrangThai == "Chưa thanh toán"
                               select db.MaDatBan).SingleOrDefault();
            return maDatBan;
        }


        [HttpPost]
        public async Task<JsonResult> ThemHoaDonChiTiet()
        {
            try
            {
                // Đọc dữ liệu từ Request.Body
                using (var reader = new System.IO.StreamReader(Request.Body))
                {
                    var body = await reader.ReadToEndAsync();  // Đọc toàn bộ nội dung body
                    dynamic data = JsonConvert.DeserializeObject(body);  // Chuyển đổi body thành đối tượng JSON

                    string maDatBan = data.maDatBan;
                    string maMA = data.maMA;
                    int soLuong = data.soLuong;
                    Console.WriteLine("Mã Đặt bàn: " + maDatBan);
                    Console.WriteLine("Mã Món ăn: " + maMA);

                    // Tiến hành xử lý dữ liệu như cũ
                    // Lấy mã hóa đơn
                    string maHoaDon = (from hd in _QLNhaHangContext.HoaDons
                                       where hd.MaDatBan == maDatBan
                                       select hd.MaHoaDon).SingleOrDefault();

                    // Lấy giá món ăn
                    double giaMonAn = (from ma in _QLNhaHangContext.MonAns
                                       where ma.MaMonAn == maMA
                                       select ma.Gia).FirstOrDefault() ?? 0.0;

                    // Kiểm tra và thêm hoặc cập nhật hóa đơn chi tiết
                    var item = _QLNhaHangContext.HoaDonChiTiets
                                .Where(hdct => hdct.MaHoaDon == maHoaDon && hdct.MaMonAn == maMA)
                                .SingleOrDefault();

                    if (item == null)
                    {
                        item = new HoaDonChiTiet
                        {
                            MaHoaDon = maHoaDon,
                            MaMonAn = maMA,
                            SoLuong = soLuong,
                            Gia = giaMonAn
                        };
                        _QLNhaHangContext.HoaDonChiTiets.Add(item);
                    }
                    else
                    {
                        item.SoLuong += soLuong;
                        _QLNhaHangContext.HoaDonChiTiets.Update(item);
                    }

                    _QLNhaHangContext.SaveChanges();

                    // Trả về JSON thành công
                    return Json(new { success = true, message = "Thêm thành công!" });
                }
            }
            catch (Exception ex)
            {
                // Trả về JSON lỗi
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        public IActionResult DSHoaDonChiTiet(string maDatBan)
        {
            // Lấy mã hóa đơn
            string maHoaDon = (from hd in _QLNhaHangContext.HoaDons
                               where hd.MaDatBan == maDatBan
                               select hd.MaHoaDon).SingleOrDefault();
            ViewData["MaDatBan"] = maDatBan;
            ViewData["MaHoaDon"] = maHoaDon;
            var dsHoaDonChiTiet = _QLNhaHangContext.HoaDonChiTiets.Include(ma => ma.MaMonAnNavigation).Where(hdct => hdct.MaHoaDon == maHoaDon).ToList();
            return PartialView("_HoaDonPartial", dsHoaDonChiTiet);
        }

        [HttpPost]
        public async Task<JsonResult> SuaSLHoaDonChiTiet()
        {
            try
            {
                // Đọc dữ liệu từ Request.Body
                using (var reader = new System.IO.StreamReader(Request.Body))
                {
                    var body = await reader.ReadToEndAsync();  // Đọc toàn bộ nội dung body
                    dynamic data = JsonConvert.DeserializeObject(body);  // Chuyển đổi body thành đối tượng JSON

                    string maDatBan = data.maDatBan;
                    string maMA = data.maMA;
                    int soLuong = data.soLuong;
                    Console.WriteLine("Mã Đặt bàn: " + maDatBan);
                    Console.WriteLine("Mã Món ăn: " + maMA);

                    // Tiến hành xử lý dữ liệu như cũ
                    // Lấy mã hóa đơn
                    string maHoaDon = (from hd in _QLNhaHangContext.HoaDons
                                       where hd.MaDatBan == maDatBan
                                       select hd.MaHoaDon).SingleOrDefault();

                    // Lấy giá món ăn
                    double giaMonAn = (from ma in _QLNhaHangContext.MonAns
                                       where ma.MaMonAn == maMA
                                       select ma.Gia).FirstOrDefault() ?? 0.0;

                    // Kiểm tra và thêm hoặc cập nhật hóa đơn chi tiết
                    var item = _QLNhaHangContext.HoaDonChiTiets
                                .Where(hdct => hdct.MaHoaDon == maHoaDon && hdct.MaMonAn == maMA)
                                .SingleOrDefault();

                    item.SoLuong = soLuong;
                    _QLNhaHangContext.HoaDonChiTiets.Update(item);
                    _QLNhaHangContext.SaveChanges();

                    // Trả về JSON thành công
                    return Json(new { success = true, message = "Thêm thành công!" });
                }
            }
            catch (Exception ex)
            {
                // Trả về JSON lỗi
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }
        public IActionResult SearchMonAn(string keyword, string maLoaiMon)
        {
            // Lấy tất cả món ăn từ cơ sở dữ liệu (dùng AsEnumerable để chuyển sang bộ nhớ client)
            var resultQuery = _QLNhaHangContext.MonAns.AsQueryable();

            // Nếu maLoaiMon không phải "Tất cả", tìm kiếm theo mã loại món ăn
            if (maLoaiMon != "Tất cả")
            {
                resultQuery = resultQuery.Where(m => m.LoaiMa == maLoaiMon);  // Lọc theo mã loại món ăn
            }

            // Chuyển toàn bộ dữ liệu từ cơ sở dữ liệu vào bộ nhớ client
            var result = resultQuery
                .AsEnumerable()  // Chuyển sang bộ nhớ client
                .Where(m => string.IsNullOrEmpty(keyword) ||
                            (RemoveDiacritics(m.TenMonAn.ToLower()).Contains(RemoveDiacritics(keyword.ToLower()))))  // Tìm theo từ khóa không phân biệt dấu và chữ hoa/chữ thường
                .ToList();

            return PartialView("_MonAnList", result);
        }

        [HttpPost]
        public async Task<JsonResult> XoaHoaDonChiTiet()
        {
            try
            {
                // Đọc dữ liệu từ Request.Body
                using (var reader = new System.IO.StreamReader(Request.Body))
                {
                    var body = await reader.ReadToEndAsync();  // Đọc toàn bộ nội dung body
                    dynamic data = JsonConvert.DeserializeObject(body);  // Chuyển đổi body thành đối tượng JSON

                    string maDatBan = data.maDatBan;
                    string maMA = data.maMA;
                    Console.WriteLine("Mã Đặt bàn: " + maDatBan);
                    Console.WriteLine("Mã Món ăn: " + maMA);

                    // Tiến hành xử lý dữ liệu như cũ
                    // Lấy mã hóa đơn
                    string maHoaDon = (from hd in _QLNhaHangContext.HoaDons
                                       where hd.MaDatBan == maDatBan
                                       select hd.MaHoaDon).SingleOrDefault();
                    ViewData["MaHoaDon"] = maHoaDon;

                    // Lấy giá món ăn
                    double giaMonAn = (from ma in _QLNhaHangContext.MonAns
                                       where ma.MaMonAn == maMA
                                       select ma.Gia).FirstOrDefault() ?? 0.0;

                    // Kiểm tra và thêm hoặc cập nhật hóa đơn chi tiết
                    var item = _QLNhaHangContext.HoaDonChiTiets
                                .Where(hdct => hdct.MaHoaDon == maHoaDon && hdct.MaMonAn == maMA)
                                .SingleOrDefault();

                    _QLNhaHangContext.HoaDonChiTiets.Remove(item);
                    _QLNhaHangContext.SaveChanges();

                    // Trả về JSON thành công
                    return Json(new { success = true, message = "Thêm thành công!" });
                }
            }
            catch (Exception ex)
            {
                // Trả về JSON lỗi
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }
        //Màn hình thanh toán
        public IActionResult ManHinhThanhToan()
        {
            return View();
        }
        //Lấy thông tin khách hàng
        public IActionResult LayThongTinKH(string maDatBan)
        {
            var ttkh = _QLNhaHangContext.DatBans.Where(db => db.MaDatBan == maDatBan).FirstOrDefault();
            return PartialView("_ThongTinKhachHang", ttkh);
        }
        //
        public IActionResult XemDSMonAnDaGoi(string maDatBan)
        {
            // Lấy mã hóa đơn
            string maHoaDon = (from hd in _QLNhaHangContext.HoaDons
                               where hd.MaDatBan == maDatBan
                               select hd.MaHoaDon).SingleOrDefault();
            ViewData["MaDatBan"] = maDatBan;
            ViewData["MaHoaDon"] = maHoaDon;
            var dsHoaDonChiTiet = _QLNhaHangContext.HoaDonChiTiets.Include(ma => ma.MaMonAnNavigation).Where(hdct => hdct.MaHoaDon == maHoaDon).ToList();
            return PartialView("_MonAnDaGoi", dsHoaDonChiTiet);
        }
        //CapNhapHoaDon
        [HttpPost]
        public async Task<JsonResult> CapNhatHoaDon()
        {
            using (var reader = new System.IO.StreamReader(Request.Body))
            {
                var body = await reader.ReadToEndAsync();  // Đọc toàn bộ nội dung body
                dynamic data = JsonConvert.DeserializeObject(body);  // Chuyển đổi body thành đối tượng JSON
                string maDatBan = data.maDatBan;
                string maMA = data.maMA;
                // Lấy mã hóa đơn dựa trên mã đặt bàn
                string maHoaDon = (from hd in _QLNhaHangContext.HoaDons
                                   where hd.MaDatBan == maDatBan
                                   select hd.MaHoaDon).SingleOrDefault();
                if (maHoaDon == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy hóa đơn tương ứng." });
                }
                // Lấy danh sách chi tiết hóa đơn dựa trên mã hóa đơn
                var dsHoaDonChiTiet = _QLNhaHangContext.HoaDonChiTiets
                                                      .Where(hdct => hdct.MaHoaDon == maHoaDon)
                                                      .ToList();
                // Tính tổng tiền cho từng chi tiết hóa đơn (Số lượng * Giá)
                double? tongTien = dsHoaDonChiTiet.Sum(hdct => hdct.SoLuong * hdct.Gia);
                var hoaDon = _QLNhaHangContext.HoaDons.Where(hd => hd.MaHoaDon == maHoaDon).FirstOrDefault();
                hoaDon.TrangThai = "Đã thanh toán";
                hoaDon.TongTien = tongTien;
                hoaDon.NgayXuatHd = DateTime.Now;
                var maBan = _QLNhaHangContext.DatBans
                            .Where(b => b.MaDatBan == maDatBan && b.NgayDatBan.Value.Date == DateTime.Now.Date)
                            .Select(b => b.MaBan)
                            .FirstOrDefault();
                var ban = _QLNhaHangContext.Bans.Where(b => b.MaBan == maBan).FirstOrDefault();
                ban.TrangThai = false;
                _QLNhaHangContext.Bans.Update(ban);
                _QLNhaHangContext.HoaDons.Update(hoaDon);
                _QLNhaHangContext.SaveChanges();
                return Json(new { success = true, message = "Xuất hóa đơn thành công!" });
            }

        }

        [HttpGet]
        public JsonResult GetDoanhThu(DateTime startDate, DateTime endDate)
        {
            var doanhThu = _QLNhaHangContext.HoaDons
                            .Where(hd => hd.NgayXuatHd.Value.Date >= startDate.Date && hd.NgayXuatHd.Value.Date <= endDate.Date)
                            .GroupBy(hd => hd.NgayXuatHd.Value.Date)
                            .Select(g => new
                            {
                                Ngay = g.Key,
                                DoanhThu = g.Sum(hd => hd.TongTien)
                            })
                            .OrderBy(d => d.Ngay)
                            .ToList()
                            .Select(d => new
                            {
                                Ngay = d.Ngay.ToString("MM/dd/yyyy"),
                                DoanhThu = d.DoanhThu
                            })
                            .ToList();

            return Json(doanhThu);
        }

        [HttpGet]
        public JsonResult GetMonAnBanChay(DateTime startDate, DateTime endDate)
        {
            var monAnBanChay = (from ct in _QLNhaHangContext.HoaDonChiTiets
                                join hd in _QLNhaHangContext.HoaDons
                                on ct.MaHoaDon equals hd.MaHoaDon
                                where hd.NgayXuatHd.Value.Date >= startDate.Date && hd.NgayXuatHd.Value.Date <= endDate.Date
                                group ct by ct.MaMonAn into groupedItems
                                select new
                                {
                                    MonAn = (from ma in _QLNhaHangContext.MonAns
                                             where ma.MaMonAn == groupedItems.Key
                                             select ma.TenMonAn).FirstOrDefault(),
                                    SoLuong = groupedItems.Sum(ct => ct.SoLuong)
                                })
                   .OrderByDescending(m => m.SoLuong)
                   .ToList();

            return Json(monAnBanChay);
        }

        //Hiện ds lịch sử hóa đơn theo ngày
        public IActionResult DSHoaDonTheoNgay(string date = null)
        {
            var selectedDate = string.IsNullOrEmpty(date) ? DateTime.Today : DateTime.Parse(date);
            ViewData["SelectedDate"] = selectedDate.ToString("MM/dd/yyyy");
            return View("DSHoaDonTheoNgay");
        }
        public IActionResult ThongTinHD(string date = null)
        {
            // Nếu không có ngày được chọn, sử dụng ngày hiện tại
            var selectedDate = string.IsNullOrEmpty(date) ? DateTime.Today : DateTime.Parse(date);
            ViewData["SelectedDate"] = selectedDate.ToString("MM/dd/yyyy");

            // Lọc danh sách hóa đơn theo ngày
            var dsHoaDonTheoNgay = _QLNhaHangContext.HoaDons
                         .Where(b => b.NgayXuatHd.Value.Date == selectedDate.Date &&
                                b.TrangThai == "Đã thanh toán")
                         .ToList();

            // Kiểm tra xem yêu cầu là một AJAX request hay không
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                // Trả về một partial view nếu là AJAX request
                return PartialView("_DSHoaDonTheoNgayPartial", dsHoaDonTheoNgay);
            }

            // Trả về view bình thường nếu không phải AJAX request
            return View(dsHoaDonTheoNgay);
        }
        //Đăng ký ca
        public IActionResult DangKyCa(string maNV, string date = null)
        {
            maNV = HttpContext.Session.GetString("MaNV");
            var selectedDate = string.IsNullOrEmpty(date) ? DateTime.Today : DateTime.Parse(date);
            ViewData["SelectedDate"] = selectedDate.ToString("MM/dd/yyyy");
            return View();
        }
        // Lấy danh sách ca theo ngày
        public IActionResult DSCaTheoNgay(string maNVien, string date = null)
        {
            maNVien = HttpContext.Session.GetString("MaNV");
            Console.WriteLine("Mã NV: " + maNVien);
            var selectedDate = string.IsNullOrEmpty(date) ? DateTime.Today : DateTime.Parse(date);
            ViewData["SelectedDate"] = selectedDate.ToString("MM/dd/yyyy");
            var layVTCT = _QLNhaHangContext.NhanViens.Where(maNV => maNV.MaNv == maNVien).FirstOrDefault();
            Console.WriteLine("Vị Trí: " + layVTCT);
            var dsCa = _QLNhaHangContext.SoLuongTrongCas.Where(ca => ca.Ngay.Date == selectedDate.Date).ToList();
            var dsCaTheoNgayVaVTCT = new List<SoLuongChiTietTrongCa>(); // Khởi tạo danh sách

            foreach (var i in dsCa)
            {
                // Lấy dữ liệu cho từng phần tử trong dsCa
                var dsCaTheoNgayVaVTCTTemp = _QLNhaHangContext.SoLuongChiTietTrongCas
                    .Include(vtct => vtct.MaViTriCvNavigation)         // Liên kết đến bảng vị trí công việc
                    .Include(sl => sl.MaQuanLyNavigation)               // Liên kết đến bảng quản lý
                    .ThenInclude(ql => ql.MaCaNavigation)              // Tiếp tục liên kết từ quản lý đến ca làm việc
                    .Where(slct => slct.MaViTriCv == layVTCT.MaViTriCv && slct.MaQuanLy == i.MaQuanLy)
                    .ToList();

                // Thêm kết quả vào danh sách chung
                dsCaTheoNgayVaVTCT.AddRange(dsCaTheoNgayVaVTCTTemp);
            }

            // Sau khi vòng lặp xong, trả về PartialView với tất cả dữ liệu đã thu thập
            return PartialView("_DSCaTheoNgayPartial", dsCaTheoNgayVaVTCT);
        }

        [HttpGet]
        public IActionResult KiemTraDangKy(string maQL, string maNV)
        {
            maNV = HttpContext.Session.GetString("MaNV");
            var ttCa = _QLNhaHangContext.SoLuongChiTietTrongCas.Include(x => x.MaQuanLyNavigation)
                .Where(slct => slct.MaQuanLyChiTiet == maQL)
                .FirstOrDefault();
            var dKyCa = _QLNhaHangContext.DangKyCas
                    .Where(dky => dky.MaQuanLy == ttCa.MaQuanLy)
                    .Count();
            int? sLTrongCa = ttCa.SoLuong;
            if (ttCa != null)
            {
                var kTraDangKyChua = _QLNhaHangContext.DangKyCas
                    .Where(dky => dky.MaQuanLy == ttCa.MaQuanLy && dky.MaNv == maNV)
                    .Any();
                if (dKyCa < sLTrongCa)
                {
                    if (kTraDangKyChua == true)
                    {
                        return Json(new { success = false, message = "Đã đăng ký" });
                    }
                }
                else if (dKyCa >= sLTrongCa)
                {
                    if (kTraDangKyChua == true)
                    {
                        return Json(new { success = false, message = "Đã đăng ký" });
                    }
                    return Json(new { success = false }); // Không đủ chỗ
                }
                // Lấy thời gian hiện tại
                DateTime currentDate = DateTime.Now.Date;

                // Lấy thời gian bắt đầu của ca
                if (ttCa.MaQuanLyNavigation != null)
                {
                    if (ttCa.MaQuanLyNavigation.Ngay.Date > currentDate.Date)
                    {
                        //sltc.Ngay.Date < DateTime.Today.AddDays(7).Date || sltc.Ngay.Date <= DateTime.Today.Date
                        DateTime ngayCa = ttCa.MaQuanLyNavigation.Ngay;
                        // Tiếp tục xử lý với ngayCa
                        //DateTime.Now > chuyenThamQuan.NgayToChuc.AddDays(-7)
                        int soNgay = (ttCa.MaQuanLyNavigation.Ngay.Date - currentDate.Date).Days;
                        if (soNgay < 7 || soNgay < 0)
                        {
                            return Json(new { success = false }); // Không cho phép đăng ký nếu ngày ca đã trễ hơn 7 ngày so với ngày hiện tại
                        }
                        return Json(new { success = true });
                    }
                    return Json(new { success = false }); // Không cho phép đăng ký nếu ngày ca đã trễ hơn 7 ngày so với ngày hiện tại
                }


                if (dKyCa < sLTrongCa)
                {
                    return Json(new { success = true }); // Đủ chỗ
                }


            }

            return Json(new { success = false }); // Không đủ chỗ
        }

        public IActionResult HienDSDKyTheoMaNV(string maNVien, string date = null)
        {
            maNVien = HttpContext.Session.GetString("MaNV");
            Console.WriteLine("Mã NV: " + maNVien);
            var selectedDate = string.IsNullOrEmpty(date) ? DateTime.Today : DateTime.Parse(date);
            ViewData["SelectedDate"] = selectedDate.ToString("MM/dd/yyyy");
            var dsDky = _QLNhaHangContext.DangKyCas.Include(ql => ql.MaQuanLyNavigation).ThenInclude(ql => ql.MaCaNavigation).Where(dky => dky.MaNv == maNVien && dky.MaQuanLyNavigation.Ngay.Date == selectedDate.Date).ToList();
            return PartialView("_DSDangKyCa", dsDky);
        }
        public string TaoMaDangKy(string maQL)
        {
            string maQLy = maQL.Substring(2, 8);

            Console.WriteLine("Mã QL: " + maQL);
            Console.WriteLine("Mã QLy: " + maQLy);

            // Lấy mã đăng ký lớn nhất theo ngày
            var maxMaDangKy = _QLNhaHangContext.DangKyCas
                .Where(dky => dky.MaDangKy.Substring(2, 8) == maQLy)
                .OrderByDescending(dky => dky.MaDangKy)
                .Select(dky => dky.MaDangKy)
                .FirstOrDefault(); // Lấy mã lớn nhất hoặc null nếu không có dữ liệu

            int soDem = 1; // Nếu không có mã nào, bắt đầu từ 001
            if (maxMaDangKy != null)
            {
                // Lấy 3 ký tự cuối (số thứ tự)
                string soThuTuStr = maxMaDangKy.Substring(10, 3);

                // Chuyển thành số và tăng 1
                if (int.TryParse(soThuTuStr, out int soThuTu))
                {
                    soDem = soThuTu + 1;
                }
            }

            // Định dạng số thứ tự thành 3 chữ số (001, 002, ..., 999)
            string soDemStr = soDem.ToString("D3");

            // Tạo mã đăng ký mới
            string maDangKy = "DK" + maQLy + soDemStr;

            return maDangKy;
        }


        public IActionResult DangKy(DangKyCa dangKyCa, string maQL, string maNV, string date = null)
        {
            var maNVien = HttpContext.Session.GetString("MaNV");
            var selectedDate = string.IsNullOrEmpty(date) ? DateTime.Today : DateTime.Parse(date);
            ViewData["SelectedDate"] = selectedDate.ToString("MM/dd/yyyy");
            Console.WriteLine(maQL);
            // Kiểm tra nếu session không tồn tại hoặc đã hết hạn
            if (string.IsNullOrEmpty(maNVien))
            {
                TempData["ThongBao"] = "Session đã hết hạn hoặc không tồn tại. Vui lòng đăng nhập lại.";
                return RedirectToAction("Logout", "Admin");
            }
            // Thêm thông tin đăng ký
            dangKyCa.MaDangKy = TaoMaDangKy(maQL);
            dangKyCa.MaNv = maNVien; // Gán mã sinh viên từ session
            dangKyCa.Ngay = DateTime.Now;
            dangKyCa.MaQuanLy = maQL;
            try
            {
                _QLNhaHangContext.DangKyCas.Add(dangKyCa);
                _QLNhaHangContext.SaveChanges();
                TempData["ThongBaoThem"] = "Đăng ký ca thành công!";
            }
            catch (Exception ex)
            {
                // Log lỗi (nếu cần)
                TempData["ThongBao"] = "Đã xảy ra lỗi trong quá trình đăng ký. Vui lòng thử lại.";
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message); // In chi tiết lỗi từ InnerException
                }
                throw new Exception(ex.Message);
            }
            Console.WriteLine(ViewData["SelectedDate"]);
            return RedirectToAction("DangKyCa", new { maNV = maNV, date = ViewData["SelectedDate"] });
            ////return RedirectToAction("DangKyCa");
        }
        [HttpGet]
        public IActionResult KiemTraDeHuy(string maQL, string maNV)
        {
            maNV = HttpContext.Session.GetString("MaNV");
            var layVTCT = _QLNhaHangContext.NhanViens.Where(nv => nv.MaNv == maNV).FirstOrDefault();
            var ttCa = _QLNhaHangContext.SoLuongChiTietTrongCas.Include(x => x.MaQuanLyNavigation)
                .Where(slct => slct.MaQuanLy == maQL && slct.MaViTriCv == layVTCT.MaViTriCv)
                .FirstOrDefault();
            if (ttCa != null)
            {
                DateTime currentDate = DateTime.Now.Date;
                // Lấy thời gian bắt đầu của ca
                if (ttCa.MaQuanLyNavigation.MaQuanLy != null)
                {
                    if (ttCa.MaQuanLyNavigation.Ngay.Date > currentDate.Date)
                    {
                        //sltc.Ngay.Date < DateTime.Today.AddDays(7).Date || sltc.Ngay.Date <= DateTime.Today.Date
                        DateTime ngayCa = ttCa.MaQuanLyNavigation.Ngay;
                        // Tiếp tục xử lý với ngayCa
                        //DateTime.Now > chuyenThamQuan.NgayToChuc.AddDays(-7)
                        int soNgay = (ttCa.MaQuanLyNavigation.Ngay.Date - currentDate.Date).Days;
                        if (soNgay < 0)
                        {
                            return Json(new { success = false, message = "Quá hạn để hủy" });
                        }
                        if (soNgay < 7)
                        {
                            return Json(new { success = false, message = "Quá hạn để hủy" }); // Không cho phép đăng ký nếu ngày ca đã trễ hơn 7 ngày so với ngày hiện tại
                        }

                        return Json(new { success = true });
                    }
                    return Json(new { success = false, message = "Quá hạn để hủy" }); // Không cho phép đăng ký nếu ngày ca đã trễ hơn 7 ngày so với ngày hiện tại
                }
                return Json(new { success = false });
            }

            return Json(new { success = false }); // Không đủ chỗ

        }
        public IActionResult HuyDangKy(string maDK, string maNV, string date = null)
        {
            var maNVien = HttpContext.Session.GetString("MaNV");
            var selectedDate = string.IsNullOrEmpty(date) ? DateTime.Today : DateTime.Parse(date);
            ViewData["SelectedDate"] = selectedDate.ToString("MM/dd/yyyy");
            // Kiểm tra nếu session không tồn tại hoặc đã hết hạn
            if (string.IsNullOrEmpty(maNVien))
            {
                TempData["ThongBao"] = "Session đã hết hạn hoặc không tồn tại. Vui lòng đăng nhập lại.";
                return RedirectToAction("Logout", "Admin");
            }
            var ttDKy = _QLNhaHangContext.DangKyCas.Where(dk => dk.MaDangKy == maDK).FirstOrDefault();
            if (ttDKy != null)
            {
                _QLNhaHangContext.DangKyCas.Remove(ttDKy);
                _QLNhaHangContext.SaveChanges();
            }
            Console.WriteLine(ViewData["SelectedDate"]);
            return RedirectToAction("DangKyCa", new { maNV = maNV, date = ViewData["SelectedDate"] });
            ////return RedirectToAction("DangKyCa");
        }


        //Xem danh sach dang ky
        public IActionResult DanhSachDangKyList(int? page, string deadTime)
        {


            var selectedDate = string.IsNullOrEmpty(deadTime) ? DateTime.Today : DateTime.Parse(deadTime);
            ViewData["SelectedDate"] = selectedDate.ToString("MM/dd/yyyy");
            // Lấy thông tin mã nhân viên và vị trí công việc từ session          
            return View();
        }

        [HttpGet]
        public IActionResult TimKiemDanhSachDangKy(int? page, string deadTime)
        {
            // Lấy thông tin ngày đã lọc từ query string (nếu có), nếu không thì sử dụng ngày hiện tại
            var selectedDate = string.IsNullOrEmpty(deadTime) ? DateTime.Today : DateTime.Parse(deadTime);
            ViewData["SelectedDate"] = selectedDate.ToString("MM/dd/yyyy");

            // Lấy thông tin mã nhân viên từ session
            var maNVien = HttpContext.Session.GetString("MaNV");
            var nhanVien = _QLNhaHangContext.NhanViens
                .Include(nv => nv.MaViTriCvNavigation)
                .Where(a => a.MaNv == maNVien) // Dùng navigation property để lấy thông tin vị trí công việc
                .FirstOrDefault();

            if (nhanVien != null)
            {
                var maViTriCv = nhanVien.MaViTriCvNavigation.MaViTriCv;
                // Kiểm tra nếu vị trí công việc là "VT010" (quản lý ca)
                if (maViTriCv == "VT010")
                {
                    // Hiển thị danh sách đăng ký nếu là quản lý ca
                    var dsDangKy = _QLNhaHangContext.DangKyCas.Where(dk => dk.MaNv == maNVien).ToList();
                    var dsQL = new List<SoLuongTrongCa>();
                    foreach (var item in dsDangKy)
                    {
                        var dsCa = _QLNhaHangContext.SoLuongTrongCas.Include(B => B.MaCaNavigation)
                            .Where(a => a.MaQuanLy == item.MaQuanLy && a.Ngay.Date == selectedDate.Date).ToList();
                        dsQL.AddRange(dsCa);
                    }
                    return PartialView("_XemDSDangKyCaContainer", dsQL);
                }
            }
            return PartialView("_XemDSDangKyCaContainer");
        }



        public IActionResult ChiTietDSDangKyCa(string maQuanLy, int? page)
        {
            if (string.IsNullOrEmpty(maQuanLy))
            {
                return NotFound("Mã Quản Lý không hợp lệ.");
            }
            int pageSize = 5; // Số lượng bản ghi mỗi trang
            int pageNumber = page ?? 1; // Trang hiện tại, mặc định là trang 1
            var maNVien = HttpContext.Session.GetString("MaNV");
            var nhanVien = _QLNhaHangContext.NhanViens.Include(nv => nv.MaViTriCvNavigation)
                .Where(a => a.MaNv == maNVien)// Dùng navigation property để lấy thông tin vị trí công việc
                .FirstOrDefault();
            if (nhanVien != null)
            {
                // Truy vấn danh sách chi tiết số lượng ca theo MaQuanLy
                var sltc = _QLNhaHangContext.DangKyCas
                .Include(c => c.MaQuanLyNavigation) // Load thông tin liên quan nếu cần
                .Include(c => c.MaNvNavigation)
                .ThenInclude(c => c.MaViTriCvNavigation)// Load thông tin vị trí công việc
                .Where(c => c.MaQuanLy == maQuanLy) // Lọc theo MaQuanLy
                .OrderBy(c => c.MaDangKy) // Sắp xếp theo thứ tự mã quản lý chi tiết
                .ToPagedList(pageNumber, pageSize);

                ViewBag.MaQuanLy = maQuanLy; // Truyền mã quản lý vào View để sử dụng (nếu cần)
                return View(sltc);
            }
            return View();
        }
    }
}




