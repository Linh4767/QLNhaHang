using System;
using System.Collections.Generic;

namespace QLNhaHang.Models
{
    public partial class Ban
    {
        public Ban()
        {
            DatBans = new HashSet<DatBan>();
            LichSuChuyenBanMaBanCuNavigations = new HashSet<LichSuChuyenBan>();
            LichSuChuyenBanMaBanMoiNavigations = new HashSet<LichSuChuyenBan>();
        }

        public string MaBan { get; set; } = null!;
        public int? SoLuongNguoi { get; set; }
        public string ViTri { get; set; } = null!;
        public bool? TrangThai { get; set; }

        public virtual ICollection<DatBan> DatBans { get; set; }
        public virtual ICollection<LichSuChuyenBan> LichSuChuyenBanMaBanCuNavigations { get; set; }
        public virtual ICollection<LichSuChuyenBan> LichSuChuyenBanMaBanMoiNavigations { get; set; }
    }
}
