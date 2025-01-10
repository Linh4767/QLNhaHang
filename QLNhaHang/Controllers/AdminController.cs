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

        public IActionResult DSBanAn(int? floor = 1, string date = null)
        {
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
                TrangThai = _QLNhaHangContext.DatBans.Any(db => db.MaBan == ban.MaBan && db.NgayDatBan.Value.Date == selectedDate.Date)
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
            calam.MaCa = TaoMaCaTuDong(); // Hoặc giữ nguyên giá trị của calam.MaCa

            ModelState.Remove("MaCa");
            // Lấy giá trị TimeSpan từ nullable
            if (!calam.ThoiGianBatDau.HasValue || !calam.ThoiGianKetThuc.HasValue)
            {
                ModelState.AddModelError("", "Thời gian bắt đầu và kết thúc không được để trống.");
                return View(calam);
            }

            TimeSpan gioBatDau = calam.ThoiGianBatDau.Value;
            TimeSpan gioKetThuc = calam.ThoiGianKetThuc.Value;
            TimeSpan thoiGianLamViec = gioKetThuc - gioBatDau;

            // Debug: In ra thời gian làm việc để kiểm tra
            Console.WriteLine("Thời gian làm việc: " + thoiGianLamViec.TotalHours + " hours");

            // Kiểm tra thời gian bắt đầu (phải sau 7:00 sáng)
            if (gioBatDau < new TimeSpan(7, 0, 0))
            {
                ModelState.AddModelError("ThoiGianBatDau", "Thời gian bắt đầu phải sau 7:00 sáng.");
            }

            // Kiểm tra thời gian kết thúc (phải trước hoặc bằng 22:30 tối)
            if (gioKetThuc > new TimeSpan(22, 30, 0))
            {
                ModelState.AddModelError("ThoiGianKetThuc", "Thời gian kết thúc không được quá 22:30 tối.");
            }

            // Kiểm tra thời gian làm việc (phải từ 4 đến 8 tiếng)
            double thoiGianLamViecInHours = thoiGianLamViec.TotalHours;
            Console.WriteLine("Thời gian làm việc (hours): " + thoiGianLamViecInHours);

            // Kiểm tra nếu là Full-time, thời gian làm việc phải đúng 8 tiếng
            if (calam.LoaiCa == "fulltime" && thoiGianLamViecInHours != 8)
            {
                ModelState.AddModelError("", "Ca làm full-time phải có thời gian làm việc đúng 8 tiếng.");
            }

            // Kiểm tra nếu là Part-time, mặc định thời gian làm việc là 4 tiếng
            if (calam.LoaiCa == "parttime" && thoiGianLamViecInHours != 4)
            {
                ModelState.AddModelError("", "Ca làm part-time phải có thời gian làm việc đúng 4 tiếng.");
            }


            // Nếu có lỗi, trả về lại view với thông báo lỗi
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).ToList();
                foreach (var error in errors)
                {
                    // Log hoặc kiểm tra chi tiết lỗi
                    Console.WriteLine(error.ErrorMessage);

                }
                // Đảm bảo giữ lại mã ca nếu có lỗi
                return View(calam);


            }


            // Nếu hợp lệ, tạo mã ca tự động và thêm vào database
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
            // Giả sử bạn lấy ca làm từ cơ sở dữ liệu
            var calam = _QLNhaHangContext.Cas.FirstOrDefault(c => c.MaCa == id);

            if (calam == null)
            {
                return NotFound();
            }

            // Trả về view với model đã có dữ liệu
            return View(calam);
        }
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
            _QLNhaHangContext.DatBans.Add(datBan);
            _QLNhaHangContext.SaveChanges();

            //thông báo khi thêm thành công
            TempData["DatBan"] = "Đặt bàn thành công!";
            return RedirectToAction("DSBanAn");
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
        //[HttpPost]
        //public async Task<IActionResult> SuaMonAn(MonAn monAn, IFormFile HinhAnh)
        //{
        //    ModelState.Remove("MaMonAn");
        //    ModelState.Remove("HinhAnh");
        //    // Lưu URL ảnh cũ
        //    var oldImageUrl = _QLNhaHangContext.MonAns
        //                    .Where(ma => ma.MaMonAn == monAn.MaMonAn)
        //                    .Select(ma => ma.HinhAnh)
        //                    .FirstOrDefault();
        //    var oldMonAn = _QLNhaHangContext.MonAns
        //                    .Where(ma => ma.MaMonAn == monAn.MaMonAn)
        //                    .FirstOrDefault();

        //    // Kiểm tra xem có ảnh mới không
        //    if (HinhAnh != null)
        //    {
        //        // Lấy đuôi file ảnh từ tên file được upload
        //        var fileExtension = Path.GetExtension(HinhAnh.FileName).ToLower();

        //        // Đổi tên ảnh theo tên món ăn (loại bỏ đuôi file gốc nếu có)
        //        var fileNameWithoutExtension = monAn.TenMonAn.Replace(" ", "-").ToLower(); // Tên món ăn, không có đuôi
        //        var fileName = fileNameWithoutExtension + fileExtension;  // Thêm đuôi file vào cuối

        //        // Tạo Cloudinary account từ thông tin cấu hình
        //        var account = new Account(
        //            _cloudinarySettings.CloudName,
        //            _cloudinarySettings.ApiKey,
        //            _cloudinarySettings.ApiSecret
        //        );
        //        var cloudinary = new Cloudinary(account);

        //        // Nếu có ảnh cũ, xóa ảnh trên Cloudinary
        //        if (!string.IsNullOrEmpty(oldImageUrl))
        //        {
        //            var oldImagePublicId = oldImageUrl.Split('/').Last().Split('?')[0];
        //            var deleteParams = new DeletionParams(oldImagePublicId);
        //            var deletionResult = await cloudinary.DestroyAsync(deleteParams);

        //            if (deletionResult.StatusCode != HttpStatusCode.OK)
        //            {
        //                // Xử lý lỗi nếu việc xóa ảnh cũ không thành công
        //                TempData["DoiTenAnh"] = "Không thể xóa ảnh cũ trên Cloudinary.";
        //                var danhMucMAList = _QLNhaHangContext.LoaiMonAns.ToList();
        //                ViewBag.danhMucMAList = new SelectList(danhMucMAList, "MaLoaiMa", "TenLoaiMa");
        //                return View(monAn);
        //            }
        //        }

        //        // Tạo stream cho file mới
        //        var fileStream = HinhAnh.OpenReadStream();
        //        var uploadParams = new ImageUploadParams()
        //        {
        //            File = new FileDescription(fileName, fileStream),
        //            Folder = "QLNhaHang",
        //            PublicId = fileNameWithoutExtension
        //        };

        //        // Thực hiện upload ảnh mới lên Cloudinary
        //        var uploadResult = await cloudinary.UploadAsync(uploadParams);

        //        // Kiểm tra kết quả upload
        //        if (uploadResult.StatusCode == HttpStatusCode.OK)
        //        {
        //            // Nếu upload thành công, lưu URL ảnh mới vào đối tượng MonAn
        //            monAn.HinhAnh = uploadResult.SecureUrl.ToString();
        //        }
        //    }
        //    else
        //    {
        //        // Nếu không có ảnh mới, giữ nguyên ảnh cũ
        //        monAn.HinhAnh = oldImageUrl;
        //        if (oldMonAn.TenMonAn != monAn.TenMonAn && !string.IsNullOrEmpty(oldImageUrl))
        //        {
        //            var account = new Account(
        //                _cloudinarySettings.CloudName,
        //                _cloudinarySettings.ApiKey,
        //                _cloudinarySettings.ApiSecret
        //            );
        //            var cloudinary = new Cloudinary(account);

        //            var oldImagePublicId = oldImageUrl
        //                                    .Split(new[] { "/image/upload/" }, StringSplitOptions.None).Last() // Lấy phần sau "/image/upload/"
        //                                    .Split('?')[0] // Loại bỏ query string (nếu có)
        //                                    .Split('.')[0]; // Loại bỏ đuôi file (.jpg, .png, ...)
        //            var newFileNameWithoutExtension = monAn.TenMonAn.Replace(" ", "-").ToLower();
        //            var renameParams = new RenameParams(oldImagePublicId, "QLNhaHang/" + newFileNameWithoutExtension);

        //            var renameResult = await cloudinary.RenameAsync(renameParams);

        //            if (renameResult.StatusCode == HttpStatusCode.OK)
        //            {
        //                oldMonAn.HinhAnh = renameResult.SecureUrl;
        //            }
        //            else
        //            {
        //                var danhMucMAList = _QLNhaHangContext.LoaiMonAns.ToList();
        //                ViewBag.danhMucMAList = new SelectList(danhMucMAList, "MaLoaiMa", "TenLoaiMa");
        //                TempData["XoaLoi"] = "Không thể xóa ảnh cũ trên Cloudinary.";
        //                return View(monAn);
        //            }
        //        }               
        //    }
        //    // Lưu monAn vào cơ sở dữ liệu (nếu cần)
        //    monAn.TenMonAn = VietHoa(monAn.TenMonAn);
        //    _QLNhaHangContext.MonAns.Update(monAn);
        //    await _QLNhaHangContext.SaveChangesAsync();

        //    // Trả về View
        //    return RedirectToAction("DanhSachMonAn_Admin");
        //}
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
                else if(oldMonAn.TenMonAn == monAn.TenMonAn && !string.IsNullOrEmpty(oldImageUrl))
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
            var query = _QLNhaHangContext.MonAns.ToList();

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
    }
}

