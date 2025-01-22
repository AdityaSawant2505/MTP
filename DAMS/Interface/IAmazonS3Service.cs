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
        Task<List<string>> UploadFilesAsync(List<IFormFile> files); // Uploads multiple files and returns their URLs
    }
}
