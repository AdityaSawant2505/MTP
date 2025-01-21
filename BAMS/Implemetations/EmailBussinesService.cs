using BAMS.Interface;
using DAMS.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAMS.Implemetations
{
    public class EmailBussinesService:IEmailBussinesService
    {
        private readonly IEmailService _emailService;
        public EmailBussinesService(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task SendEmailAsync(List<string> receivers, string subject, string message, List<IFormFile> attachments = null)
        {
            await _emailService.SendEmailAsync(receivers, subject, message, attachments);
        }
    }
}
