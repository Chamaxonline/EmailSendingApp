
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace ModelLibrary
{
    public class Message
    {
        public List<MailboxAddress> To { get; set; }
        public List<string> Tu { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public IFormFileCollection Attachments { get; set; }

        public Message(IEnumerable<string> to,List<string> tu, string subject, string content)
        {
            To = new List<MailboxAddress>();
            To.AddRange(to.Select(x => new MailboxAddress("",x)));
            Tu = tu;
            Subject = subject;
            Content = content;
        }
    }

}
