using System.Net;

namespace ViralWave.Application.Helpers;

public static class FTPHelper
{
    public static bool UploadFileToFtp(FileInfo file, string folderName, string ftpUrl, string ftpUserName, string ftpPassword)
    {
        var retryingCount = 0;

        while (retryingCount < 5)
        {
            try
            {
                var urlPath = string.Concat(ftpUrl, "/", file.Name);
                if (ftpUserName == "TempFiles")
                {
                    urlPath = string.Concat(ftpUrl, "/", folderName, "/", file.Name);
                }

                FtpWebRequest request =
                    (FtpWebRequest)WebRequest.Create(urlPath);
                request.Method = WebRequestMethods.Ftp.UploadFile;

                // Get network credentials.
                request.Credentials =
                    new NetworkCredential(ftpUserName, ftpPassword);

                // Read the file's contents into a byte array.
                byte[] bytes = System.IO.File.ReadAllBytes(file.FullName);

                // Write the bytes into the request stream.
                request.ContentLength = bytes.Length;
                using (Stream request_stream = request.GetRequestStream())
                {
                    request_stream.Write(bytes, 0, bytes.Length);
                    request_stream.Close();
                }

                retryingCount = 5;
            }
            catch (Exception ex)
            {
                retryingCount++;
                if (ex.Message.Contains("file not found"))
                {
                    CreateFtpDirectory(ftpUrl, ftpUserName, ftpPassword);
                }
                if (retryingCount == 5)
                    return false;
            }
        }
        return true;
    }
    
    private static void CreateFtpDirectory(string directoryUrl, string ftpUserName, string ftpPassword)
    {
        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(directoryUrl);
        request.Method = WebRequestMethods.Ftp.MakeDirectory;
        request.Credentials = new NetworkCredential(ftpUserName, ftpPassword);

        using FtpWebResponse response = (FtpWebResponse)request.GetResponse();

    }
}