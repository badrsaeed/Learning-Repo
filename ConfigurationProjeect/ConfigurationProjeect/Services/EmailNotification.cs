using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using ViralWave.Application.Helpers;
using ViralWave.Application.Interfaces.Services;
using ViralWave.Application.Models;

namespace ViralWave.Infrastructure.Services
{
    public class EmailNotification : IEmailNotification
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailNotification> _logger;


        public EmailNotification(IConfiguration configuration, ILogger<EmailNotification> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public bool Notify(Message model)
        {
            bool b = false;
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage();
                m.From = new MailAddress(model.From);
                m.Subject = model.Subject;
                m.Body = model.Body;
                m.IsBodyHtml = true;


                if (!String.IsNullOrEmpty(model.To))
                {
                    string[] ar = model.To.Split(',');
                    foreach (string address in ar)
                        m.To.Add(address);
                }

                if (!String.IsNullOrEmpty(model.CC))
                {
                    string[] ar = model.CC.Split(',');
                    foreach (string address in ar)
                        m.CC.Add(address);
                }

                if (!String.IsNullOrEmpty(model.BCC))
                {
                    string[] ar = model.BCC.Split(',');
                    foreach (string address in ar)
                        m.Bcc.Add(address);
                }

                if (!String.IsNullOrEmpty(model.Attachment))
                {
                    string[] ar = model.Attachment.Split(',');
                    foreach (string file in ar)
                    {
                        Attachment item = new Attachment(file);
                        m.Attachments.Add(item);
                        //item.Dispose();
                    }
                }

                System.Net.Mail.SmtpClient emailClient = new System.Net.Mail.SmtpClient(model.SmtpClient);
                System.Net.NetworkCredential SMTPUserInfo = new System.Net.NetworkCredential(model.Username, model.Password);
                emailClient.UseDefaultCredentials = false;
                emailClient.Credentials = SMTPUserInfo;
                emailClient.EnableSsl = true;
                emailClient.Port = model.Port == 0 ? Convert.ToInt32(_configuration["EmailSettings:Port"]) : model.Port;

                emailClient.Send(m);
                b = true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error In sending email =  and the inner exception is {ex.InnerException}");
                throw;
                //b = false;
            }
            return b;
        }

        public void SendMail(string MailTo, string MailCc,string Subject, string MailTitle, string MailContent,  string AttachmentPath = "", string Other = "", string MailBCC = "")
        {
            //ConfigurationManager.AppSettings["FromMail"]

            Message mailMessage = new Message();
            mailMessage.From = _configuration["EmailSettings:From"];
            mailMessage.To = MailTo;
            mailMessage.CC = MailCc;
            mailMessage.BCC = MailBCC;
            mailMessage.Subject = Subject;
            mailMessage.Attachment = AttachmentPath;
            mailMessage.SmtpClient = _configuration["EmailSettings:SmtpClient"];
            mailMessage.Username = _configuration["EmailSettings:Username"];
            mailMessage.Password = _configuration["EmailSettings:Password"];
            mailMessage.Body = ConstantVariables.mailBody(MailTitle, MailContent, Other);

            try
            {
                Notify(mailMessage);
            }catch(Exception ex)
            {
                _logger.LogError($"Error while sending mail, {ex.Message}");
                throw;
            }
        }

        public void SendMail(string MailTo, string MailCc, string Subject, string MailTitle, string MailContent, string AttachmentPath, string userName, bool isNotification)
        {
            Message mailMessage = new Message();
            mailMessage.From = _configuration["EmailSettings:From"];
            mailMessage.To = MailTo;
            mailMessage.CC = MailCc;
            mailMessage.Subject = Subject;
            mailMessage.Attachment = AttachmentPath;
            mailMessage.SmtpClient = _configuration["EmailSettings:SmtpClient"];
            mailMessage.Username = _configuration["EmailSettings:Username"];
            mailMessage.Password = _configuration["EmailSettings:Password"];
            mailMessage.Body = ConstantVariables.mailBody(MailTitle, MailContent, userName, true);

            Notify(mailMessage);
        }

    }
}
