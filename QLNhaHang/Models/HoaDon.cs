using System;
using System.Collections.Generic;

namespace QLNhaHang.Models
{
    public partial class HoaDon
    {
        public HoaDon()
        {
            HoaDonChiTiets = new HashSet<HoaDonChiTiet>();
        }

        public string MaHoaDon { get; set; } = null!;
        public string MaDatBan { get; set; } = null!;
        public DateTime? NgayXuatHd { get; set; }
        public double? TongTien { get; set; }

        public virtual DatBan MaDatBanNavigation { get; set; } = null!;
        public virtual ICollection<HoaDonChiTiet> HoaDonChiTiets { get; set; }
    }
}
