using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ViralWave.Application.Interfaces.Services;
using ViralWave.Application.Models;
namespace ViralWave.Infrastructure.Services
{
    public class FTPService : IFTPService
    {
        private readonly ILogger<FTPService> _logger;
        public FTPService(ILogger<FTPService> logger)
        {
            _logger = logger;
        }

        public async Task UploadFileToFTPAsync(FTPModel ftpModel, IFormFile file)
        {
            string batchId = Guid.NewGuid().ToString();
            try
            {
                var directoryPath = $"{ftpModel.Host}/{batchId}/{file.FileName}";

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(directoryPath));
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(ftpModel.Username, ftpModel.Password);
                request.UseBinary = true;

                //using (FileStream localFileStream = new FileStream("localFilePath", FileMode.Open))
                using (Stream requestStream = await request.GetRequestStreamAsync())
                {
                    await file.CopyToAsync(requestStream);
                }

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                _logger.LogInformation($"Batch:{batchId} Uploaded Successfully, status {response.StatusDescription}");
                response.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error uploading Batch:{batchId} :: {ex.Message}");
            }
        }


    }
}