using System;
using System.Collections.Generic;

namespace QLNhaHang.Models
{
    public partial class LoaiMonAn
    {
        public LoaiMonAn()
        {
            MonAns = new HashSet<MonAn>();
        }

        public string MaLoaiMa { get; set; } = null!;
        public string TenLoaiMa { get; set; } = null!;

        public virtual ICollection<MonAn> MonAns { get; set; }
    }
}
