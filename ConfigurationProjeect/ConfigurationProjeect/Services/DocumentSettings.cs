using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViralWave.Application.Helpers
{
    public static class DocumentSettings
    {
        public static async Task<string> UploadFile(string path,IFormFile file, string folderName)
        {
            //Get Folder Path
            string folderPath = Path.Combine(path,folderName);
            //Checking if exist or not
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            //Create File Name and make it unique
            string fileName = $"{Guid.NewGuid()}{file.FileName}";

            //Get File Path
            string filePath = Path.Combine(folderPath, fileName);

            //Craete and Save the file using FileStream Class
            using var fs = new FileStream(filePath, FileMode.Create);

            //Copy the file stream to 
            await file.CopyToAsync(fs);

            return fileName;
        }

        public static async Task<byte[]> GetImage(string path,int folderName, string fileName)
        {
            var filePath =Path.Combine(path, Convert.ToString(folderName), fileName);
            if (File.Exists(filePath))
            {
                return File.ReadAllBytes(filePath);
            }
            return new byte[0]; 
        }
    }
}
