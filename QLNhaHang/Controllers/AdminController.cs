using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLNhaHang.Models;
using System.Globalization;
using System.Text;
using X.PagedList;
using X.PagedList.Extensions;


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
            var query = _QLNhaHangContext.ViTriCongViecs.AsQueryable();

            // Kiểm tra nếu có từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(vtcv => vtcv.TenViTriCv.Contains(searchQuery));
            }

            // Phân trang và sắp xếp
            var dsTimKiem = query.OrderBy(vtcv => vtcv.MaViTriCv).ToPagedList(pageNumber, pageSize);

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
        //Tìm kiếm tên danh mục
        [HttpGet]
        public IActionResult TimKiemLoaiMonAn(string tuKhoa)
        {
            var dsTimKiem = string.IsNullOrEmpty(tuKhoa) ? _QLNhaHangContext.LoaiMonAns.ToList() : _QLNhaHangContext.LoaiMonAns.Where(lma => lma.TenLoaiMa.Contains(tuKhoa)).ToList();
            return PartialView("_LoaiMATableContainer", dsTimKiem);
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

            var dsBan = _QLNhaHangContext.Bans
                         .Where(b => b.ViTri.Contains($"Lầu {floor}"))
                         .ToList();

            return View(dsBan);
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
            var chiTiet = _QLNhaHangContext.SoLuongChiTietTrongCas.Where(c =>c.MaQuanLyChiTiet == maQuanLyChiTiet).FirstOrDefault();
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
                TempData["ThongBaoLoi"] = "Không tìm thấy chi tiết cần cập nhật.";
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

    }
}


