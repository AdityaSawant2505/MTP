﻿using BAMS.Interface;
using DAMS.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAMS.Implemetations
{
    public class AmazonBussinessService : IAmazonBussinessService
    {
        private readonly IAmazonS3Service _amazonS3Service; 
        public AmazonBussinessService(IAmazonS3Service amazonS3Service)
        {
            _amazonS3Service = amazonS3Service;
        }
        public async Task<List<string>> UploadFilesAsync(List<IFormFile> files)
        {
            return await _amazonS3Service.UploadFilesAsync(files);
        }
    }
}
