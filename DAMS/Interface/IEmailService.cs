using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAMS.Interface
{
    public interface IEmailService
    {
        //Task SendEmailAsync(List<string> receivers, string subject, string message, List<IFormFile> attachments = null);
        //Jayant
        Task SendEmailAsync(List<string> receivers, string subject, string message, List<IFormFile> attachments = null);

    }
}
