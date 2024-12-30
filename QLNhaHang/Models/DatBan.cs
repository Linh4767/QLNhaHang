using System;
using System.Collections.Generic;

namespace QLNhaHang.Models
{
    public partial class DatBan
    {
        public DatBan()
        {
            HoaDons = new HashSet<HoaDon>();
            LichSuChuyenBans = new HashSet<LichSuChuyenBan>();
        }

        public string MaDatBan { get; set; } = null!;
        public string MaBan { get; set; } = null!;
        public int? SoNguoiDi { get; set; }
        public string? TenKh { get; set; }
        public string? Sdt { get; set; }
        public DateTime? NgayDatBan { get; set; }

        public virtual Ban MaBanNavigation { get; set; } = null!;
        public virtual ICollection<HoaDon> HoaDons { get; set; }
        public virtual ICollection<LichSuChuyenBan> LichSuChuyenBans { get; set; }
    }
}
