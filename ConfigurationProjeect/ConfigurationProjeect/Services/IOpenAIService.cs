using Microsoft.AspNetCore.Http;
using ViralWave.Application.DTOs;

namespace ViralWave.Application.Interfaces.Implementation
{
    public interface IOpenAIService
    {
        Task<object> CreateAssistantWithFiles(string platformName, List<IFormFile> files);
        Task<string?> GetAssistantIdFromDb(string platformName);
        Task<string> ChatWithAssistant(string platformName, string userMessage, string authHeader);
        Task<string> RequestGeminiAI(RequestGeminiAIDto model);

    }
}
