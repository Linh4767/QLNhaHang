using MailKit.Net.Imap;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using QLNhaHang.Models;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;


namespace QLNhaHang.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly QLNhaHangContext _QLNhaHangContext;

        public HomeController(ILogger<HomeController> logger, QLNhaHangContext qLNhaHangContext)
        {
            _logger = logger;
            _QLNhaHangContext = qLNhaHangContext;
        }

        public IActionResult TrangChu()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult FormGuiEmailDatBan()
        {
            return PartialView("_FormGuiEmailDatBan");
        }
        [HttpPost]
        public async Task<IActionResult> SendEmail(GuiEmailDatBan model)
        {
            ModelState.Remove("Message");
            if (ModelState.IsValid)
            {
                try
                {
                    var email = new MimeMessage();
                    email.From.Add(new MailboxAddress(model.Name, model.Email));
                    email.To.Add(new MailboxAddress("Restaurant Admin", "ministorelaravel@gmail.com"));
                    email.Subject = "Yêu Cầu Đặt Bàn Mới";
                    model.Message = string.IsNullOrEmpty(model.Message) ? "Không có ghi chú" : model.Message;
                    email.Body = new TextPart("plain")
                    {
                        Text = $@"
                    Yêu Cầu Đặt Bàn Mới:
                    Tên Khách Hàng: {model.Name}
                    Email: {model.Email}
                    Số Điện Thoại: {model.Phone}
                    Ngày Đặt Bàn: {model.Date:yyyy-MM-dd}
                    Thời Gian Đặt Bàn: {model.Time}
                    Số Người Đi: {model.People}
                    Ghi Chú: {model.Message}"
                    };

                    using (var smtp = new SmtpClient())
                    {
                        smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                        smtp.Authenticate("ministorelaravel@gmail.com", "z e n g t h d v w b g u f c t u");
                        await smtp.SendAsync(email);
                        smtp.Disconnect(true);
                    }

                    return Json(new { success = true, message = "Yêu cầu đặt bàn của bạn gửi đi thành công!" });
                }
                catch
                {
                    return Json(new { success = false, message = "Có lỗi xảy ra trong quá trình gửi. Vui lòng thử lại" });
                }
            }

            return Json(new { success = false, message = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại!" });
        }
        // Action to get unread emails from Gmail
        [HttpGet]
        public async Task<IActionResult> GetUnreadEmails()
        {
            try
            {
                using (var client = new ImapClient())
                {
                    // Connect to Gmail's IMAP server
                    await client.ConnectAsync("imap.gmail.com", 993, SecureSocketOptions.SslOnConnect);
                    await client.AuthenticateAsync("ministorelaravel@gmail.com", "z e n g t h d v w b g u f c t u");

                    // Open inbox and fetch unread emails
                    var inbox = client.Inbox;
                    await inbox.OpenAsync(FolderAccess.ReadOnly);
                    var unreadMessages = inbox.Fetch(0, -1, MessageSummaryItems.Envelope | MessageSummaryItems.Flags)
                                               .Where(x => !(x.Flags.HasValue && x.Flags.Value.HasFlag(MessageFlags.Seen)))
                                               .ToList();

                    var unreadEmailList = new List<object>();

                    foreach (var message in unreadMessages)
                    {
                        try
                        {
                            var email = await inbox.GetMessageAsync(message.Index);
                            string emailBody = email.TextBody.Replace("\r\n", "<br/>").Replace("\n", "<br/>");
                            unreadEmailList.Add(new
                            {
                                Subject = email.Subject,
                                From = email.From.ToString(),
                                Date = email.Date,
                                Body = emailBody,
                                Status = "Pending" // No match check here
                            });
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error fetching email {message.Index}: {ex.Message}");
                        }
                    }

                    return Json(new { success = true, data = unreadEmailList ?? new List<object>() });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while fetching emails." });
            }
        }

        // Action to check if reservation matches
        // Action to check if reservation matches
        [HttpPost]
        public IActionResult CheckReservationMatch(string emailBody)
        {
            try
            {
                // Trích xuất thông tin từ nội dung email
                var emailDetails = ExtractEmailDetails(emailBody);

                if (emailDetails != null)
                {                    
                    bool isMatch = CheckDatBanMatch(emailDetails); // Kiểm tra khớp với dữ liệu đặt bàn
                    if (isMatch)
                    {
                        // Nếu khớp, đánh dấu email là "đã đọc"
                        MarkEmailAsRead(emailBody);
                        return Json(new { success = true, message = "Xác nhận thành công và email đã được đánh dấu là đã đọc." });
                    }
                    return Json(new { success = false, message = "Không tìm thấy thông tin đặt bàn khớp." });
                }

                return Json(new { success = false }); // Trả về false nếu không có dữ liệu phù hợp
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during reservation check: {ex.Message}");
                return Json(new { success = false, message = "Error processing the reservation." });
            }
        }

        // Method to extract details from email body (name, phone, etc.)
        private GuiEmailDatBan? ExtractEmailDetails(string emailBody)
        {
            try
            {
                Console.WriteLine($"Email Body: {emailBody}"); // In ra email body đã được làm sạch
                string cleanEmailBody = Regex.Replace(emailBody, "<.*?>", string.Empty);
                Console.WriteLine($"Clean Email Body: {cleanEmailBody}"); // In ra email body đã được làm sạch
                // Cập nhật các mẫu regex để xử lý thẻ HTML đúng cách
                var nameMatch = Regex.Match(emailBody, @"Tên Khách Hàng:\s*(.*?)<br/>", RegexOptions.IgnoreCase);
                var phoneMatch = Regex.Match(emailBody, @"Số Điện Thoại:\s*(\d{10})<br/>", RegexOptions.IgnoreCase);
                var dateMatch = Regex.Match(emailBody, @"Ngày Đặt Bàn:\s*(\d{4}-\d{2}-\d{2})<br/>", RegexOptions.IgnoreCase);
                var timeMatch = Regex.Match(emailBody, @"Thời Gian Đặt Bàn:\s*(\d{2}:\d{2})<br/>", RegexOptions.IgnoreCase);
                var peopleMatch = Regex.Match(emailBody, @"Số Người Đi:\s*(\d+)<br/>", RegexOptions.IgnoreCase);
                Console.WriteLine($"Name Match: {nameMatch.Value}");
                Console.WriteLine($"Phone Match: {phoneMatch.Value}");
                Console.WriteLine($"Date Match: {dateMatch.Value}");
                Console.WriteLine($"Time Match: {timeMatch.Value}");
                Console.WriteLine($"People Match: {peopleMatch.Value}");

                // Kiểm tra sự thành công của các regex matches
                if (nameMatch.Success && phoneMatch.Success && dateMatch.Success && timeMatch.Success && peopleMatch.Success)
                {
                    DateTime date = DateTime.ParseExact(dateMatch.Groups[1].Value, "yyyy-MM-dd", null);
                    string time = timeMatch.Groups[1].Value;
                    int people = int.Parse(peopleMatch.Groups[1].Value);

                    return new GuiEmailDatBan
                    {
                        Name = nameMatch.Groups[1].Value,
                        Phone = phoneMatch.Groups[1].Value,
                        Date = date,
                        Time = time,
                        People = people
                    };
                }

                return null; // Trả về null nếu một trong các trường không khớp
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting email details: {ex.Message}");
                return null;
            }
        }

        // Method to check if reservation matches
        private bool CheckDatBanMatch(GuiEmailDatBan emailDetails)
        {
            try
            {
                var emailTimeSpan = TimeSpan.Parse(emailDetails.Time); // Chuyển thành TimeSpan
                var datBan = _QLNhaHangContext.DatBans
     .Where(d => d.TenKh == emailDetails.Name && d.Sdt == emailDetails.Phone &&
                 d.NgayDatBan.Value.Date == emailDetails.Date.Date && // Kiểm tra ngày đặt bàn
                 d.NgayDatBan.Value.Hour == emailTimeSpan.Hours &&  // So sánh giờ
                 d.NgayDatBan.Value.Minute == emailTimeSpan.Minutes) // So sánh phút
     .FirstOrDefault();


                if (datBan == null)
                {
                    Console.WriteLine("No matching datBan found. Please check the input details.");
                    return false; // Hoặc thực hiện logic khác nếu cần
                }
                // Chỉ so sánh phần ngày (không so sánh phần thời gian)
                else if (datBan.NgayDatBan.HasValue && TimeSpan.TryParse(emailDetails.Time, out TimeSpan emailTime))
                {
                    Console.WriteLine($"Email Time: {emailDetails.Date.Hour} - {emailDetails.Date.Minute}");
                    Console.WriteLine($"ĐB Time: {datBan.NgayDatBan.Value.Hour} - {datBan.NgayDatBan.Value.Minute}");
                    // Chỉ so sánh phần ngày của các đối tượng DateTime
                    string databaseDateString = datBan.NgayDatBan.Value.ToString("yyyy-MM-dd");
                    string emailDateString = emailDetails.Date.ToString("yyyy-MM-dd");
                    Console.WriteLine("hh: " + databaseDateString);
                    Console.WriteLine("hh1: " + emailDateString);
                    bool isDateMatch = databaseDateString == emailDateString;


                    // Tạo DateTime từ email's time và so sánh toàn bộ thời gian (giờ, phút)
                    // Kiểm tra lại toàn bộ thời gian (kể cả giờ và phút)
                    DateTime emailDateTime = emailDetails.Date.Date.Add(emailTime);
                    DateTime databaseDateTime = datBan.NgayDatBan.Value;

                    Console.WriteLine($"DB Time: {databaseDateTime.TimeOfDay}, Email Time: {emailDateTime.TimeOfDay}");

                    bool isTimeMatch = emailDateTime.Hour == databaseDateTime.Hour &&
                                       emailDateTime.Minute == databaseDateTime.Minute;

                    Console.WriteLine($"Time Match: {isTimeMatch}, DB Time: {databaseDateTime.TimeOfDay}, Email Time: {emailDateTime.TimeOfDay}");


                    return isDateMatch && isTimeMatch && datBan.TenKh == emailDetails.Name && datBan.Sdt == emailDetails.Phone;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking DatBan match: {ex.Message}");
                return false;
            }
        }
        private void MarkEmailAsRead(string emailBody)
        {
            using (var client = new MailKit.Net.Imap.ImapClient())
            {
                client.Connect("imap.gmail.com", 993, true); // Kết nối tới IMAP
                client.Authenticate("ministorelaravel@gmail.com", "z e n g t h d v w b g u f c t u");

                // Chọn hộp thư (vd: "INBOX")
                var inbox = client.GetFolder("[Gmail]/All Mail");
                inbox.Open(MailKit.FolderAccess.ReadWrite);
                if (inbox.IsOpen)
                {
                    Console.WriteLine("Hộp thư [Gmail]/All Mail đã mở thành công.");
                }
                else
                {
                    Console.WriteLine("Không thể mở hộp thư [Gmail]/All Mail.");
                    return; // Dừng xử lý nếu không mở được
                }
                // Tìm email chứa nội dung khớp với emailBody
                string cleanedBody = Regex.Replace(emailBody, @"<.*?>", string.Empty).Trim();
                var query = MailKit.Search.SearchQuery.BodyContains(cleanedBody);
                var results = inbox.Search(query);
                Console.WriteLine(results);
                if (results.Any())
                {
                    // Đánh dấu email đầu tiên khớp là "đã đọc"
                    inbox.AddFlags(results.First(), MailKit.MessageFlags.Seen, true);
                }

                client.Disconnect(true);
            }
        }
    }
}
