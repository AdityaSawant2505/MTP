using BAMS.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailBussinesService _emailManagerService;

        public EmailController(IEmailBussinesService emailManagerService)
        {
            _emailManagerService = emailManagerService;
        }

        [HttpPost("send-email")]
        public async Task<IActionResult> SendEmail([FromForm] List<string> receivers,
            [FromForm] string subject,
            [FromForm] string message,
            [FromForm] List<IFormFile> attachments = null)
        {
            try
            {
                await _emailManagerService.SendEmailAsync(receivers, subject, message, attachments);
                return Ok("Email sent successfully!");
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred while sending the email.");
            }
        }
    }
}
