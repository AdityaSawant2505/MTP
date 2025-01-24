using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using DAMS.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class AmazonS3Service : IAmazonS3Service
{
    private readonly string _bucketName;
    private readonly string _accessKey;
    private readonly string _secretKey;
    private readonly RegionEndpoint _region;

    public AmazonS3Service(IConfiguration configuration)
    {
        // Fetch AWS credentials from configuration
        _bucketName = configuration["AWS:BucketName"];
        _accessKey = configuration["AWS:AccessKey"];
        _secretKey = configuration["AWS:SecretKey"];
        _region = RegionEndpoint.GetBySystemName(configuration["AWS:Region"]);
    }

    public async Task<List<string>> UploadFilesAsync(List<IFormFile> files)
    {
        using (var s3Client = new AmazonS3Client(_accessKey, _secretKey, _region))
        {
            var transferUtility = new TransferUtility(s3Client);
            var uploadedFileUrls = new List<string>();

            foreach (var file in files)
            {
                try
                {
                    // Generate a unique key for the file in the S3 bucket
                    var key = $"practfolder1/{file.FileName}";

                    using (var memoryStream = new MemoryStream())
                    {
                        // Copy the file data to the memory stream
                        await file.CopyToAsync(memoryStream);
                        memoryStream.Seek(0, SeekOrigin.Begin);

                        var uploadRequest = new TransferUtilityUploadRequest
                        {
                            InputStream = memoryStream,
                            Key = key,
                            BucketName = _bucketName
                            // Remove the CannedACL property
                        };

                        // Upload the file to S3
                        await transferUtility.UploadAsync(uploadRequest);

                        // Add only the file name (key) to the database
                        uploadedFileUrls.Add(file.FileName);
                    }
                }
                catch (AmazonS3Exception ex)
                {
                    Console.WriteLine($"AWS S3 Error: {ex.Message}");
                    throw;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"General Error: {ex.Message}");
                    throw;
                }
            }

            return uploadedFileUrls;
        }
    }

    public async Task<string> UpdateFileAsync(IFormFile file, string existingFileKey)
    {
        existingFileKey = $"practfolder1/{existingFileKey}";
        using (var s3Client = new AmazonS3Client(_accessKey, _secretKey, _region))
        {
            var transferUtility = new TransferUtility(s3Client);

            try
            {
                // Generate a new key (new name) for the file
                var newFileName = $"{file.FileName}";
                var newFileKey = $"practfolder1/{newFileName}";

                using (var memoryStream = new MemoryStream())
                {
                    // Copy the new file data to the memory stream
                    await file.CopyToAsync(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        InputStream = memoryStream,
                        Key = newFileKey, // Use the new file key
                        BucketName = _bucketName,
                    };

                    // Upload the new file to S3
                    await transferUtility.UploadAsync(uploadRequest);

                    // Optionally, delete the old file
                    if (!string.IsNullOrEmpty(existingFileKey))
                    {
                        await s3Client.DeleteObjectAsync(_bucketName, existingFileKey);
                    }

                    // Return the new file key
                    return newFileKey;
                }
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"AWS S3 Error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
                throw;
            }
        }
    }

    public async Task<bool> DeleteFileAsync(string fileKey)
    {
        fileKey = $"practfolder1/{fileKey}";

        using (var s3Client = new AmazonS3Client(_accessKey, _secretKey, _region))
        {
            try
            {
                var deleteRequest = new DeleteObjectRequest
                {
                    BucketName = _bucketName,
                    Key = fileKey
                };

                await s3Client.DeleteObjectAsync(deleteRequest);

                Console.WriteLine($"File {fileKey} deleted successfully.");
                return true;
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"AWS S3 Error while deleting file: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error while deleting file: {ex.Message}");
                throw;
            }
        }
    }

    public string GetFileFullPath(string fileKey)
    {
        var bucketUrl = $"https://{_bucketName}.s3.{_region.SystemName}.amazonaws.com";
        return $"{bucketUrl}/{fileKey}";
    }

}
