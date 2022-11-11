using EmailService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmailAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailSender _emailSender;

        public EmailController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        [HttpPost("SendEmail")]
        public async Task<ActionResult> SendEmail(MessageViewModel messageViewModel)
        {
           // var files = Request.Form.Files.Any() ? Request.Form.Files : new FormFileCollection();
            var addressList = messageViewModel.EmailAddressList.ToArray();
            // var message = new Message(new string[] { "chamathran@gmail.com" }, "Test email", "This is the content from our email.");
            var message = new Message(addressList, messageViewModel.Emails, messageViewModel.Subject, messageViewModel.Content);
            await _emailSender.SendEmailAsync(message);
            return Ok();

        }
    }
}
