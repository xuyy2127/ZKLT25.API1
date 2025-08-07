using System.Net;
using System.Net.Mail;
using ZKLT25.API.Helper.JsonFile;

namespace ZKLT25.API.Helper.Email
{
    public static class EmailHelper
    {
        private static readonly bool _isOpen = JsonFileHelper.GetKeyValue("EmailSetting:IsOpen").ToBool();
        private static readonly bool _ssl = JsonFileHelper.GetKeyValue("EmailSetting:IsSSL").ToBool();
        private static readonly string _smtpServer = JsonFileHelper.GetKeyValue("EmailSetting:SMTP");
        private static readonly int _smtpPort = JsonFileHelper.GetKeyValue("EmailSetting:Port").ToInt32();
        private static readonly string _senderEmail = JsonFileHelper.GetKeyValue("EmailSetting:SendEmail");
        private static readonly string _senderPassword = JsonFileHelper.GetKeyValue("EmailSetting:SendPwd");

        /// <summary>
        /// 发送邮箱
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="toEmails"></param>
        public static void SendEmail(string title, string content, List<string> toEmails)
        {
            if (!_isOpen)
            {
                return;
            }

            if (!toEmails.Any())
            {
                return;
            }

            var isHtml = true;

            // 创建 SMTP 客户端
            using var client = new SmtpClient(_smtpServer)
            {
                Port = _smtpPort,
                Credentials = new NetworkCredential(_senderEmail, _senderPassword),
                EnableSsl = _ssl,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            // 创建邮件消息
            using var message = new MailMessage
            {
                From = new MailAddress(_senderEmail),
                Subject = title,
                Body = content,
                IsBodyHtml = isHtml
            };

            // 添加收件人
            foreach (var item in toEmails)
            {
                message.To.Add(item);
            }

            try
            {
                // 发送邮件
                client.Send(message);
            }
            catch (Exception e)
            {
                var a = e.Message;
            }
        }
    }
}
