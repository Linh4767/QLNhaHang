using System;
using System.Collections.Generic;

namespace QLNhaHang.Models
{
    public partial class ViTriCongViec
    {
        public ViTriCongViec()
        {
            NhanViens = new HashSet<NhanVien>();
            SoLuongChiTietTrongCas = new HashSet<SoLuongChiTietTrongCa>();
        }

        public string MaViTriCv { get; set; } = null!;
        public string TenViTriCv { get; set; } = null!;

        public virtual ICollection<NhanVien> NhanViens { get; set; }
        public virtual ICollection<SoLuongChiTietTrongCa> SoLuongChiTietTrongCas { get; set; }
    }
}
