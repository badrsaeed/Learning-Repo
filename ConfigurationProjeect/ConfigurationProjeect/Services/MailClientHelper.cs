using System.Net.Mail;
using System.Net;
using ViralWave.Application.Models;

namespace ViralWave.Application.Helpers
{
    public static class MailClientHelper
    {
        public static void SendMail(MailMessageModel mail, string displayName)
        {
            var m = new System.Net.Mail.MailMessage();
            m.From = new MailAddress(mail.From, displayName);
            m.Subject = mail.Subject;
            m.Body = mail.Body;

            m.IsBodyHtml = true;

            if (!string.IsNullOrEmpty(mail.To))
            {
                string[] ar = mail.To.Split(',');
                foreach (string address in ar)
                    m.To.Add(address);
            }

            if (!string.IsNullOrEmpty(mail.CC))
            {
                string[] ar = mail.CC.Split(',');
                foreach (string address in ar)
                    m.CC.Add(address);
            }

            if (!string.IsNullOrEmpty(mail.BCC))
            {
                string[] ar = mail.BCC.Split(',');
                foreach (string address in ar)
                    m.Bcc.Add(address);
            }

            if (!string.IsNullOrEmpty(mail.Attachment))
            {
                string[] ar = mail.Attachment.Split(',');
                foreach (string file in ar)
                {
                    var item = new Attachment(file);
                    m.Attachments.Add(item);
                }
            }

            using var emailClient = new SmtpClient(mail.SmtpClient)
            {
                Credentials = new NetworkCredential(mail.Username, mail.Password),
                EnableSsl = true,
                Port = mail.Port == 0 ? 587 : mail.Port,
            };
            emailClient.Send(m);

        }
    }
}
