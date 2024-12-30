using System;
using System.Collections.Generic;

namespace QLNhaHang.Models
{
    public partial class TaiKhoan
    {
        public string TaiKhoan1 { get; set; } = null!;
        public string? MatKhau { get; set; }
        public string MaNv { get; set; } = null!;

        public virtual NhanVien MaNvNavigation { get; set; } = null!;
    }
}
