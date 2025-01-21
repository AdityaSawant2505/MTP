//using DAMS.Interface;
//using Microsoft.Extensions.Configuration;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Mail;
//using System.Net;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;

//namespace DAMS.Implementations
//{
//    public class EmailService : IEmailService
//    {
//        private readonly IConfiguration _configuration;
//        public EmailService(IConfiguration configuration)
//        {
//            _configuration = configuration;
//        }

//        //public async Task SendEmailAsync(List<string> receivers, string subject, string message, List<IFormFile> attachments = null)
//        //{
//        //    try
//        //    {
//        //        var server = _configuration["SMTP:Server"];
//        //        var port = int.Parse(_configuration["SMTP:Port"]);
//        //        var senderEmail = _configuration["SMTP:SenderEmail"];
//        //        var senderPassword = _configuration["SMTP:SenderPassword"];

//        //        using (var smtpClient = new SmtpClient(server, port))
//        //        {
//        //            smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);
//        //            smtpClient.EnableSsl = true;

//        //            foreach (var receiver in receivers)
//        //            {
//        //                using (var email = new MailMessage
//        //                {
//        //                    From = new MailAddress(senderEmail),
//        //                    Subject = subject,
//        //                    Body = message,
//        //                    IsBodyHtml = true
//        //                })
//        //                {
//        //                    email.To.Add(receiver);

//        //                    if (attachments != null && attachments.Any())
//        //                    {
//        //                        foreach (var attachment in attachments)
//        //                        {
//        //                            using (var stream = new MemoryStream())
//        //                            {
//        //                                await attachment.CopyToAsync(stream);

//        //                                stream.Position = 0;
//        //                                var mailAttachment = new Attachment(stream, attachment.FileName);
//        //                                email.Attachments.Add(mailAttachment);
//        //                            }
//        //                        }
//        //                    }

//        //                    await smtpClient.SendMailAsync(email);
//        //                }
//        //            }
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        throw ex;
//        //    }
//        //}



//        //Jayant
//        public async Task SendEmailAsync(List<string> receivers, string subject, string message, List<IFormFile> attachments = null)

//        {
//            try
//            {
//                var server = _configuration["SMTP:Server"];
//                var port = int.Parse(_configuration["SMTP:Port"]);
//                var senderEmail = _configuration["SMTP:SenderEmail"];
//                var senderPassword = _configuration["SMTP:SenderPassword"];

//                using (var smtpClient = new SmtpClient(server, port))
//                {
//                    smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);
//                    smtpClient.EnableSsl = true;

//                    foreach (var receiver in receivers)
//                    {
//                        using (var email = new MailMessage
//                        {
//                            From = new MailAddress(senderEmail),
//                            Subject = subject,
//                            Body = message,
//                            IsBodyHtml = true
//                        })
//                        {
//                            email.To.Add(receiver);

//                            if (attachments != null && attachments.Any())
//                            {
//                                foreach (var attachment in attachments)
//                                {
//                                    var stream = new MemoryStream();
//                                    await attachment.CopyToAsync(stream);
//                                    stream.Position = 0; // Reset stream position to the beginning
//                                    var mailAttachment = new Attachment(stream, attachment.FileName);
//                                    email.Attachments.Add(mailAttachment);
//                                }
//                            }

//                            await smtpClient.SendMailAsync(email);
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                throw ex; // Consider logging the exception instead of rethrowing it as is.
//            }
//        }


//    }
//}

//New code
using DAMS.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DAMS.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(List<string> receivers, string subject, string message, List<IFormFile> attachments = null)
        {
            try
            {
                var server = _configuration["SMTP:Server"];
                var port = int.Parse(_configuration["SMTP:Port"]);
                var senderEmail = _configuration["SMTP:SenderEmail"];
                var senderPassword = _configuration["SMTP:SenderPassword"];

                using (var smtpClient = new SmtpClient(server, port))
                {
                    smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);
                    smtpClient.EnableSsl = true;

                    foreach (var receiver in receivers)
                    {
                        using (var email = new MailMessage
                        {
                            From = new MailAddress(senderEmail),
                            Subject = subject,
                            Body = message,
                            IsBodyHtml = true
                        })
                        {
                            email.To.Add(receiver);

                            if (attachments != null && attachments.Any())
                            {
                                foreach (var attachment in attachments)
                                {
                                    var stream = new MemoryStream();
                                    await attachment.CopyToAsync(stream);
                                    stream.Position = 0; // Reset stream position to the beginning
                                    var mailAttachment = new Attachment(stream, attachment.FileName);
                                    email.Attachments.Add(mailAttachment);
                                }
                            }

                            await smtpClient.SendMailAsync(email);
                            _logger.LogInformation("Email sent to {receiver} successfully.", receiver);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending the email.");
                throw;
            }
        }
    }
}





