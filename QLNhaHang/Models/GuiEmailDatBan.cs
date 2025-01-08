using System.ComponentModel.DataAnnotations;

namespace QLNhaHang.Models
{
    public class GuiEmailDatBan
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public int People { get; set; }
        [Required(AllowEmptyStrings = true)] // Cho phép trường trống nhưng không gây lỗi
        public string Message { get; set; } = "Không có ghi chú"; // Gán giá trị mặc định
    }
}
