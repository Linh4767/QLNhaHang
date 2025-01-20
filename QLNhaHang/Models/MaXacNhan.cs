using System;
using System.Collections.Generic;

namespace QLNhaHang.Models
{
    public partial class MaXacNhan
    {
        public int Id { get; set; }
        public string MaXacNhan1 { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime? NgayDatBan { get; set; }
        public string? TenKh { get; set; }
        public string? TrangThai { get; set; }
        public int? SoNguoiDi { get; set; }
        public string? Sdt { get; set; }
        public DateTime? NgayTao { get; set; }
    }
}
