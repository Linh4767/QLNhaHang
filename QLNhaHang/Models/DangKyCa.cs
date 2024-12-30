using System;
using System.Collections.Generic;

namespace QLNhaHang.Models
{
    public partial class DangKyCa
    {
        public string MaQuanLy { get; set; } = null!;
        public string MaDangKy { get; set; } = null!;
        public string MaNv { get; set; } = null!;
        public DateTime? Ngay { get; set; }

        public virtual NhanVien MaNvNavigation { get; set; } = null!;
        public virtual SoLuongTrongCa MaQuanLyNavigation { get; set; } = null!;
    }
}
