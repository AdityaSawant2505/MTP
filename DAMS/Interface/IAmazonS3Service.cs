using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAMS.Interface
{
    public interface IAmazonS3Service
    {
        Task<List<string>> UploadFilesAsync(List<IFormFile> files);
        Task<string> UpdateFileAsync(IFormFile file, string existingFileKey);
        Task<bool> DeleteFileAsync(string fileKey);
    }
}
