using System;
using System.Collections.Generic;

namespace QLNhaHang.Models
{
    public partial class SoLuongChiTietTrongCa
    {
        public string MaQuanLy { get; set; } = null!;
        public string MaQuanLyChiTiet { get; set; } = null!;
        public string? MaViTriCv { get; set; }
        public int? SoLuong { get; set; }

        public virtual SoLuongTrongCa MaQuanLyNavigation { get; set; } = null!;
        public virtual ViTriCongViec? MaViTriCvNavigation { get; set; }
    }
}
