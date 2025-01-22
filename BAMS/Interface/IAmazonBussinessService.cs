using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAMS.Interface
{
    public interface IAmazonBussinessService
    {
        Task<List<string>> UploadFilesAsync(List<IFormFile> files); // Uploads multiple files and returns their URLs

    }
}
