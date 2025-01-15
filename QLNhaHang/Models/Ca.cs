using System;
using System.Collections.Generic;

namespace QLNhaHang.Models
{
    public partial class Ca
    {
        public Ca()
        {
            SoLuongTrongCas = new HashSet<SoLuongTrongCa>();
        }

        public string MaCa { get; set; } = null!;
        public TimeSpan? ThoiGianBatDau { get; set; }
        public TimeSpan? ThoiGianKetThuc { get; set; }
        public string LoaiCa { get; set; } = null!;

        public virtual ICollection<SoLuongTrongCa> SoLuongTrongCas { get; set; }
    }
}
