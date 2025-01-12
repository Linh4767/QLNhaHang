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
using System.Net.Mail;
using System.Net;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using System.Globalization;


namespace QLNhaHang.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly QLNhaHangContext _QLNhaHangContext;

        public HomeController(ILogger<HomeController> logger, QLNhaHangContext QLNhaHangContext)
        {
            _logger = logger;
            _QLNhaHangContext = QLNhaHangContext;
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
        [HttpPost]
        public IActionResult CheckReservationMatch(string emailBody)
        {
            try
            {
                // Trích xuất thông tin từ nội dung email
                var emailDetails = ExtractEmailDetails(emailBody);

                if (emailDetails != null)
                {
                    MarkEmailAsRead(emailBody);
                    // Kiểm tra xem khách hàng đã có trong danh sách chờ chưa
                    string confirmationCode = GenerateConfirmationCode();
                    string ngayDatBanString = emailDetails.Date.ToString("dd/MM/yyyy") + " " + emailDetails.Time;
                    DateTime ngayDatBan = DateTime.ParseExact(ngayDatBanString, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                    var waitingListEntry = _QLNhaHangContext.MaXacNhans
                        .FirstOrDefault(x => x.Email == emailDetails.Email && x.Sdt == emailDetails.Phone && x.NgayDatBan == ngayDatBan && x.TrangThai == "Chưa xác nhận");

                    if (waitingListEntry == null)
                    {
                        // Tạo mã xác nhận

                        // Thêm khách hàng vào danh sách chờ trong cơ sở dữ liệu
                        var confirmationRecord = new MaXacNhan
                        {
                            Email = emailDetails.Email,
                            MaXacNhan1 = confirmationCode,
                            NgayDatBan = ngayDatBan,
                            SoNguoiDi = emailDetails.People,
                            TenKh = emailDetails.Name,
                            Sdt = emailDetails.Phone,
                            TrangThai = "Chưa xác nhận"
                        };

                        _QLNhaHangContext.MaXacNhans.Add(confirmationRecord);
                        _QLNhaHangContext.SaveChanges();  // Lưu vào cơ sở dữ liệu

                        // Gửi email xác nhận
                        SendWaitingListEmail(emailDetails, confirmationCode);

                        return Json(new { success = true, message = "Khách hàng đã được thêm vào danh sách chờ và email xác nhận đã được gửi." });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Khách hàng đã có trong danh sách chờ." });
                    }
                }

                return Json(new { success = false, message = "Không thể trích xuất thông tin từ email." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during reservation check: {ex.Message}");
                return Json(new { success = false, message = "Lỗi khi xử lý thông tin đặt bàn." });
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
                var emailMatch = Regex.Match(emailBody, @"Email:\s*([\w\.-]+@[\w\.-]+\.\w+)<br/>", RegexOptions.IgnoreCase);
                Console.WriteLine($"Name Match: {nameMatch.Value}");
                Console.WriteLine($"Phone Match: {phoneMatch.Value}");
                Console.WriteLine($"Date Match: {dateMatch.Value}");
                Console.WriteLine($"Time Match: {timeMatch.Value}");
                Console.WriteLine($"People Match: {peopleMatch.Value}");

                // Kiểm tra sự thành công của các regex matches
                if (nameMatch.Success && phoneMatch.Success && dateMatch.Success && timeMatch.Success && peopleMatch.Success && emailMatch.Success)
                {
                    DateTime date = DateTime.ParseExact(dateMatch.Groups[1].Value, "yyyy-MM-dd", null);
                    string time = timeMatch.Groups[1].Value;
                    int people = int.Parse(peopleMatch.Groups[1].Value);

                    return new GuiEmailDatBan
                    {
                        Name = nameMatch.Groups[1].Value,
                        Phone = phoneMatch.Groups[1].Value,
                        Email = emailMatch.Groups[1].Value,
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
        private string GenerateConfirmationCode()
        {
            var random = new Random();
            // Tạo mã ngẫu nhiên có độ dài 6 ký tự (bạn có thể thay đổi độ dài nếu muốn)
            var code = Path.GetRandomFileName().Replace(".", "").Substring(0, 6);
            return code;
        }

        private void SendWaitingListEmail(GuiEmailDatBan emailDetails, string confirmationCode)
        {
            var confirmationLink = Url.Action("Confirm", "Home", new { code = confirmationCode }, protocol: Request.Scheme);
            try
            {
                // Nội dung email
                string subject = "Xác nhận vào danh sách chờ";
                string body = $@"
            <html>
                <body>
                    <p>Kính gửi {emailDetails.Name},</p>
                    <p>Cảm ơn quý khách đã đặt bàn tại nhà hàng của chúng tôi. 
                    Hiện tại, đơn đặt bàn của quý khách đã được thêm vào danh sách chờ. 
                    Vui lòng nhấn vào nút dưới đây để xác nhận đặt bàn.</p>

                    <p><strong>Thông tin đặt bàn:</strong></p>
                    <p>
                        Tên khách hàng: {emailDetails.Name}<br>
                        Số điện thoại: {emailDetails.Phone}<br>
                        Thời gian: {emailDetails.Time} ngày {emailDetails.Date.ToString("dd/MM/yyyy tt")}<br>
                        Số người: {emailDetails.People} người<br>
                    </p>

                    <p>Vui lòng nhấn vào liên kết dưới đây để xác nhận đặt bàn của bạn:</p>
                    
                   <a href='{confirmationLink}' style='background-color: #4CAF50; color: white; padding: 15px 32px; text-align: center; text-decoration: none; display: inline-block; font-size: 16px; border-radius: 4px;'>Xác nhận đặt bàn</a>
                   <p>Trân trọng,<br>Nhà hàng ABC</p>
                </body>
            </html>";

                // Gửi email
                var mail = new MimeMessage();
                mail.From.Add(new MailboxAddress(string.Empty, "ministorelaravel@gmail.com"));
                mail.To.Add(new MailboxAddress(string.Empty, emailDetails.Email)); // Thêm email khách hàng
                mail.Subject = subject;
                mail.Body = new TextPart("html") { Text = body };

                using (var smtp = new SmtpClient())
                {
                    smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls); // Sử dụng TLS
                    smtp.Authenticate("ministorelaravel@gmail.com", "z e n g t h d v w b g u f c t u"); // Xác thực người gửi
                    smtp.Send(mail); // Gửi email
                    smtp.Disconnect(true); // Ngắt kết nối

                    Console.WriteLine("Email đã được gửi thành công.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi gửi email: {ex.Message}");
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

        // Hàm kiểm tra trạng thái "đã đọc" của email
        private bool IsEmailMarkedAsRead(string emailBody)
        {
            try
            {
                using (var client = new MailKit.Net.Imap.ImapClient())
                {
                    client.Connect("imap.gmail.com", 993, true); // Kết nối tới IMAP
                    client.Authenticate("ministorelaravel@gmail.com", "z e n g t h d v w b g u f c t u");

                    var inbox = client.GetFolder("[Gmail]/All Mail");
                    inbox.Open(MailKit.FolderAccess.ReadWrite);

                    // Tìm email chứa nội dung khớp với emailBody
                    var query = MailKit.Search.SearchQuery.BodyContains(emailBody);
                    var results = inbox.Search(query);

                    if (results.Any())
                    {
                        // Lấy thông tin email từ kết quả tìm kiếm
                        var email = inbox.GetMessage(results.First());

                        // Kiểm tra nếu email không còn trong INBOX, tức là đã đọc
                        var inboxFolder = client.GetFolder("INBOX");
                        inboxFolder.Open(MailKit.FolderAccess.ReadOnly);

                        var inboxResults = inboxFolder.Search(MailKit.Search.SearchQuery.BodyContains(emailBody));

                        if (!inboxResults.Any())
                        {
                            // Nếu email không còn trong INBOX, nghĩa là đã được đọc
                            return true;
                        }
                    }

                    client.Disconnect(true);
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking email read status: {ex.Message}");
                return false;
            }
        }
        private void SendConfirmationEmail(string customerEmail, string customerName)
        {
            try
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("Restaurant Admin", "ministorelaravel@gmail.com"));
                emailMessage.To.Add(new MailboxAddress(customerName, customerEmail));
                emailMessage.Subject = "Đặt bàn thành công";

                // Nội dung email
                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $"<p>Chào {customerName},</p><p>Chúng tôi xin thông báo rằng bạn đã đặt bàn thành công.</p><p>Chúc bạn một ngày tuyệt vời tại nhà hàng của chúng tôi!</p>"
                };

                emailMessage.Body = bodyBuilder.ToMessageBody();

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    client.Authenticate("ministorelaravel@gmail.com", "z e n g t h d v w b g u f c t u");
                    client.Send(emailMessage);
                    client.Disconnect(true);
                }

                Console.WriteLine("Email gửi thành công");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending confirmation email: {ex.Message}");
            }
        }
        [HttpGet("home/confirm")]
        public IActionResult Confirm(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                ViewBag.Message = "Mã xác nhận không hợp lệ. Vui lòng kiểm tra lại.";
                return View();
            }

            var record = _QLNhaHangContext.MaXacNhans.FirstOrDefault(x => x.MaXacNhan1 == code);

            if (record == null)
            {
                ViewBag.Message = "Mã xác nhận không tồn tại. Vui lòng kiểm tra lại.";
            }
            else
            {
                record.TrangThai = "Đã xác nhận";
                _QLNhaHangContext.SaveChanges();
                ViewBag.Message = "Xác nhận thành công! Cảm ơn bạn đã sử dụng dịch vụ.";
            }

            return View();
        }
        public IActionResult WaitingList()
        {
            var dsCho = _QLNhaHangContext.MaXacNhans.ToList();
            return View("~/Views/Admin/WaitingList.cshtml", dsCho);
        }
        [HttpPost]
        public IActionResult XacNhanDatBan(string tenKH, string sdt, int soNguoiDi, DateTime ngayDatBan)
        {
            var dsDatBan = _QLNhaHangContext.DatBans.Where(db => db.TenKh == tenKH && db.Sdt == sdt && db.SoNguoiDi == soNguoiDi && db.NgayDatBan == ngayDatBan).FirstOrDefault();
            if (dsDatBan != null)
            {
                return Json(new { success = true, message = "Thông tin này đã được đặt bàn!" });
            }
            else
            {
                return Json(new { success = false, message = "Thông tin này chưa được đặt bàn." });
            }
        }

    }
}
