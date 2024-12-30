using System;
using System.Collections.Generic;

namespace QLNhaHang.Models
{
    public partial class MonAn
    {
        public MonAn()
        {
            HoaDonChiTiets = new HashSet<HoaDonChiTiet>();
        }

        public string? LoaiMa { get; set; }
        public string MaMonAn { get; set; } = null!;
        public string TenMonAn { get; set; } = null!;
        public string? HinhAnh { get; set; }
        public double? Gia { get; set; }
        public string? MoTa { get; set; }
        public string? TrangThai { get; set; }

        public virtual LoaiMonAn? LoaiMaNavigation { get; set; }
        public virtual ICollection<HoaDonChiTiet> HoaDonChiTiets { get; set; }
    }
}
