using System;
using System.Collections.Generic;

namespace QLNhaHang.Models
{
    public partial class LichSuChuyenBan
    {
        public string MaChuyenBan { get; set; } = null!;
        public string? MaDatBan { get; set; }
        public string? MaBanCu { get; set; }
        public string? MaBanMoi { get; set; }
        public DateTime? ThoiGianChuyen { get; set; }
        public string MaNv { get; set; } = null!;
        public string? LyDoChuyen { get; set; }

        public virtual Ban? MaBanCuNavigation { get; set; }
        public virtual Ban? MaBanMoiNavigation { get; set; }
        public virtual DatBan? MaDatBanNavigation { get; set; }
        public virtual NhanVien MaNvNavigation { get; set; } = null!;
    }
}
