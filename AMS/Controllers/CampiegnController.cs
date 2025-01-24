using BAMS.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampiegnController : ControllerBase
    {
        private readonly IAmazonBussinessService _amazonBussinessService;
        public CampiegnController(IAmazonBussinessService amazonBussinessService)
        {
            _amazonBussinessService = amazonBussinessService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFiles([FromForm] List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest("No files uploaded.");
            }

            try
            {
                // Call the UploadFilesAsync method of the service
                List<string> uploadedFileUrls = await _amazonBussinessService.UploadFilesAsync(files);

                // Return the list of URLs as a response
                return Ok(uploadedFileUrls);
            }
            catch (Exception ex)
            {
                // Handle any exceptions and return a server error
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPut("UpdateFileExample")]
        public async Task UpdateFileExample(IFormFile file, string existingFileKey)
        {

            try
            {
                string updatedKey = await _amazonBussinessService.UpdateFileAsync(file, existingFileKey);
                Console.WriteLine($"File updated successfully: {updatedKey}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating file: {ex.Message}");
            }
        }


        [HttpDelete("DeleteFile")]
        public async Task DeleteFile(string fileKey)
        {
            try
            {
                bool isDeleted = await _amazonBussinessService.DeleteFileAsync(fileKey);
                if (isDeleted)
                {
                    Console.WriteLine($"File {fileKey} was deleted.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting file: {ex.Message}");
            }
        }


    }
}
