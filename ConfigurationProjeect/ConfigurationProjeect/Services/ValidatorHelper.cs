using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ViralWave.Application.Interfaces.Helpers;

namespace ViralWave.Application.Helpers
{
    public class ValidatorHelper : IValidatorHelper
    {
        private readonly IConfiguration _configuration;

        public ValidatorHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public bool IsValidDates(DateTime startDate, DateTime endDate)
        {
            DateTime MinSqlDateTime = new DateTime(1753, 1, 1);
            DateTime MaxSqlDateTime = new DateTime(9999, 12, 31);

            if (startDate < MinSqlDateTime || startDate > DateTime.Now)
                return false;
            if (endDate > MaxSqlDateTime)
                return false;
            return true;
        }

        public bool IsValidExcel(IFormFile excelFile)
        {
            const string pattern = @"\.(xlsx|xls)$";
            int excelFileSize = _configuration.GetValue<int>("MaxFileSize:Excel");
            
            return Regex.IsMatch(excelFile.FileName, pattern) && (excelFile.Length <= excelFileSize);
        }

        public  bool IsValidSong(IFormFile song)
        {
           const string pattern = @"\.(mp3|wav|flac)$";
           int maxSongSize = _configuration.GetValue<int>("MaxFileSize:Track"); 
           
           return Regex.IsMatch(song.FileName, pattern) && (song.Length <= maxSongSize);
        }
        
        public  bool IsValidImage(IFormFile image)
        {
            const string pattern = @"\.(jpg|jpeg|png|webp)$";
            int maxImageSize = _configuration.GetValue<int>("MaxFileSize:Image");
            
            return (Regex.IsMatch(image.FileName, pattern)) && (image.Length <= maxImageSize);
        }

        public bool IsValidArabicWord(string word)
        {
            const string pattern = @"^(?=.*[\u0600-\u06FF])[\u0600-\u06FF0-9\s\(\-\&~\.\)]*$";
            return Regex.IsMatch(word, pattern);
        }

        public bool IsValidEnglishWord(string word)
        {
            const string pattern = @"^(?=.*[a-zA-Z])[a-zA-Z0-9\s\(\'\-\&~\.\)]*$";
            return Regex.IsMatch(word, pattern);
        }
        
        public bool IsValidArabicAndEnglishWord(string word)
        {
            const string pattern = @"^[\u0621-\u064A\u0660-\u0669a-zA-Z\s\-,|]+$";
            return Regex.IsMatch(word, pattern);
        }
        public bool IsValidDigits(string number)
        {
            const string pattern = @"^\d*$";
            return Regex.IsMatch(number, pattern);
        }

        public bool IsValidUPC(string upc)
        {
            const string pattern = @"^(VW-\d+|\d+)$";
            return Regex.IsMatch(upc, pattern);
        }

        public bool IsValidClineAndPline(string word)
        {
            const string pattern = @"^\d{4} [a-zA-Z0-9&.-]+( [a-zA-Z0-9&.-]+)*$";
            return Regex.IsMatch(word, pattern);
        }

        public bool IsValidFullName(string fullName)
        {
            const string pattern = "^[A-Za-z]+ [A-Za-z]+$";
            return Regex.IsMatch(fullName, pattern);
        }

        public bool IsValidISRC(string isrc)
        {
            const string pattern = @"^[a-zA-Z0-9\s]+$";
            return Regex.IsMatch(isrc, pattern);
        }
        
        public bool ContainsPath(string word)
        {
            const string pattern = @"[\\/]";
            
            return Regex.IsMatch(word, pattern);
        }

        public bool IsValidNumbersOnly(string upc)
        {
            const string pattern = @"^\d*";
            return Regex.IsMatch(upc, pattern);
        }
        
    }
}
