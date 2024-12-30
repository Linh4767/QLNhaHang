using System;
using System.Collections.Generic;

namespace QLNhaHang.Models
{
    public partial class NhanVien
    {
        public NhanVien()
        {
            DangKyCas = new HashSet<DangKyCa>();
            InverseMaQuanLyNavigation = new HashSet<NhanVien>();
            LichSuChuyenBans = new HashSet<LichSuChuyenBan>();
            TaiKhoans = new HashSet<TaiKhoan>();
        }

        public string MaNv { get; set; } = null!;
        public string TenNv { get; set; } = null!;
        public string? HinhAnh { get; set; }
        public string Cccd { get; set; } = null!;
        public string? GioiTinh { get; set; }
        public string? Sdt { get; set; }
        public string? Email { get; set; }
        public string? DiaChi { get; set; }
        public string? MaViTriCv { get; set; }
        public string? TrangThai { get; set; }
        public string? MaQuanLy { get; set; }
        public bool? ThuViec { get; set; }
        public DateTime? NgayVaoLam { get; set; }
        public string? MaBhyt { get; set; }

        public virtual NhanVien? MaQuanLyNavigation { get; set; }
        public virtual ViTriCongViec? MaViTriCvNavigation { get; set; }
        public virtual ICollection<DangKyCa> DangKyCas { get; set; }
        public virtual ICollection<NhanVien> InverseMaQuanLyNavigation { get; set; }
        public virtual ICollection<LichSuChuyenBan> LichSuChuyenBans { get; set; }
        public virtual ICollection<TaiKhoan> TaiKhoans { get; set; }
    }
}
