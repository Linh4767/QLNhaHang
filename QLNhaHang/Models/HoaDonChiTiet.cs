using System;
using System.Collections.Generic;

namespace QLNhaHang.Models
{
    public partial class HoaDonChiTiet
    {
        public string MaHoaDon { get; set; } = null!;
        public string MaMonAn { get; set; } = null!;
        public int? SoLuong { get; set; }
        public double? Gia { get; set; }

        public virtual HoaDon MaHoaDonNavigation { get; set; } = null!;
        public virtual MonAn MaMonAnNavigation { get; set; } = null!;
    }
}
