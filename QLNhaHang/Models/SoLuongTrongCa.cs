using System;
using System.Collections.Generic;

namespace QLNhaHang.Models
{
    public partial class SoLuongTrongCa
    {
        public SoLuongTrongCa()
        {
            DangKyCas = new HashSet<DangKyCa>();
            SoLuongChiTietTrongCas = new HashSet<SoLuongChiTietTrongCa>();
        }

        public string MaQuanLy { get; set; } = null!;
        public string MaCa { get; set; } = null!;
        public DateTime Ngay { get; set; }
        public int? SoLuongToiDa { get; set; }

        public virtual Ca MaCaNavigation { get; set; } = null!;
        public virtual ICollection<DangKyCa> DangKyCas { get; set; }
        public virtual ICollection<SoLuongChiTietTrongCa> SoLuongChiTietTrongCas { get; set; }
    }
}
