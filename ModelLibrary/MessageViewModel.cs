using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLibrary
{
    public class MessageViewModel
    {
        public string Subject { get; set; }
        public string Content { get; set; }
        public List<string> EmailAddressList { get; set; }
        public List<string> Emails { get; set; }

    }
}
