
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using ModelLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailboxAddress = MimeKit.MailboxAddress;

namespace EmailService
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration _emailConfig;
        public EmailSender(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }
        public async Task SendEmailAsync(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            await SendAsync(emailMessage);
        }
               

        private static string MeetingRequestString(string from, List<string> toUsers, string subject, string desc, string location, DateTime startTime, DateTime endTime, int? eventID = null, bool isCancel=false)
        {
            StringBuilder str = new StringBuilder();

            str.AppendLine("BEGIN:VCALENDAR");
            str.AppendLine("PRODID:-//Microsoft Corporation//Outlook 12.06 MIMEDIR//EN");
            str.AppendLine("VERSION:2.0");
            str.AppendLine(string.Format("METHOD:{0}", (isCancel ? "CANCEL" : "REQUEST")));
            str.AppendLine("BEGIN:VEVENT");

            str.AppendLine(string.Format("DTSTART:{0:yyyyMMddTHHmmssZ}", startTime.ToUniversalTime()));
            str.AppendLine(string.Format("DTSTAMP:{0:yyyyMMddTHHmmss}", DateTime.Now));
            str.AppendLine(string.Format("DTEND:{0:yyyyMMddTHHmmssZ}", endTime.ToUniversalTime()));
            str.AppendLine(string.Format("LOCATION: {0}", location));
            str.AppendLine(string.Format("UID:{0}", (eventID.HasValue ? "blablabla" + eventID : Guid.NewGuid().ToString())));
            str.AppendLine(string.Format("DESCRIPTION:{0}", desc.Replace("\n", "<br>")));
            str.AppendLine(string.Format("X-ALT-DESC;FMTTYPE=text/html:{0}", desc.Replace("\n", "<br>")));
            str.AppendLine(string.Format("SUMMARY:{0}", subject));

            str.AppendLine(string.Format("ORGANIZER;CN=\"{0}\":MAILTO:{1}", from, from));
            str.AppendLine(string.Format("ATTENDEE;CN=\"{0}\";RSVP=TRUE:mailto:{1}", string.Join(",", toUsers), string.Join(",", toUsers)));

            str.AppendLine("BEGIN:VALARM");
            str.AppendLine("TRIGGER:-PT15M");
            str.AppendLine("ACTION:DISPLAY");
            str.AppendLine("DESCRIPTION:Reminder");
            str.AppendLine("END:VALARM");
            str.AppendLine("END:VEVENT");
            str.AppendLine("END:VCALENDAR");

            return str.ToString();
        }
        private MimeMessage CreateEmailMessage(Message message)
        {
            try
            {

            
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(message.Subject, _emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
           // emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = string.Format(message.Content) };
           // var bodyBuilder = new BodyBuilder { HtmlBody = string.Format("<h2 style='color:red;'>{0}</h2>", message.Content) };
            var bodyBuilder = new BodyBuilder { HtmlBody = string.Format(message.Content) };

            var cal = MeetingRequestString(_emailConfig.From, message.Tu, message.Subject, "desc", "location", DateTime.Now, DateTime.Now.AddHours(1), 1, false);
            byte[] bytes = Encoding.ASCII.GetBytes(cal);
            bodyBuilder.Attachments.Add("invite.ics",bytes);
            //if (message.Attachments != null && message.Attachments.Any())
            //{
            //    byte[] fileBytes;
            //    foreach (var attachment in message.Attachments)
            //    {
            //        using (var ms = new MemoryStream())
            //        {
            //            attachment.CopyTo(ms);
            //            fileBytes = ms.ToArray();
            //        }
            //        bodyBuilder.Attachments.Add(attachment.FileName, fileBytes, ContentType.Parse(attachment.ContentType));
            //    }
            //}

        


            emailMessage.Body = bodyBuilder.ToMessageBody();

           

            return emailMessage;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private async Task SendAsync(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, SecureSocketOptions.StartTls);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_emailConfig.UserName, _emailConfig.Password);
                   await client.SendAsync(mailMessage);
                }
                catch (Exception ex)
                {
                    //log an error message or throw an exception or both.
                    throw;
                }
                finally
                {
                   await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }
    }
}
