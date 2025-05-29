using Microsoft.AspNetCore.Mvc;
using ViralWave.Application.DTOs;
using ViralWave.Application.Interfaces.Implementation;

namespace ViralWave.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpenAiAssistantController : ControllerBase
    {
        private readonly IOpenAIService _openAiService;
        private readonly ILogger<OpenAiAssistantController> _logger;

        public OpenAiAssistantController(IOpenAIService openAiService, ILogger<OpenAiAssistantController> logger)
        {
            _openAiService = openAiService;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAssistant([FromForm] string platformName, [FromForm] List<IFormFile> files)
        {
            if (string.IsNullOrEmpty(platformName) || files == null || files.Count == 0)
                return BadRequest("Platform name and files are required.");

            var result = await _openAiService.CreateAssistantWithFiles(platformName, files);
            return Ok(result);
        }

        [HttpGet("{platformName}")]
        public async Task<IActionResult> GetAssistantId(string platformName)
        {
            var assistantId = await _openAiService.GetAssistantIdFromDb(platformName);

            if (string.IsNullOrEmpty(assistantId))
                return NotFound($"No assistant found for platform '{platformName}'");

            return Ok(new { Platform = platformName, AssistantId = assistantId });
        }
        [HttpPost("chat")]
        public async Task<IActionResult> ChatWithAssistant([FromBody] ChatRequestDto chatRequest)
        {
            if (string.IsNullOrEmpty(chatRequest.PlatformName) || string.IsNullOrEmpty(chatRequest.Message))
                return BadRequest("Platform name and message are required.");

            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader == null || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized("Token is missing or invalid.");
            }
            var reply = await _openAiService.ChatWithAssistant(chatRequest.PlatformName, chatRequest.Message, authHeader);

            return Ok(new { reply });
        }


        [HttpPost("requestGeminiAI")]
        public async Task<IActionResult> RequestGeminiAI([FromBody] RequestGeminiAIDto model)
        {
            try
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                {
                    return Unauthorized("Token is missing or invalid.");
                }
                _logger.LogInformation($"Getting AI response : {model}");
                var response = await _openAiService.RequestGeminiAI(model);
                _logger.LogInformation($"Response from AI: {response}");
                return Ok(new { reply = response });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in RequestGeminiAI: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
