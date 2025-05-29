using Microsoft.Extensions.Caching.Memory;
using System.Text;
using ViralWave.Application.Helpers;
using ViralWave.Application.Interfaces.Implementation;
using ViralWave.Infrastructure.Services;
using JsonDocument = System.Text.Json.JsonDocument;

namespace ViralWave.Infrastructure.Implementation
{
    public class OpenAIService : IOpenAIService
    {
        private readonly string apiKey =
            "";

        private readonly string uploadUrl = "https://api.openai.com/v1/files";
        private readonly string assistantUrl = "https://api.openai.com/v1/assistants";
        private readonly IViralWaveContextProcedures _viralWaveContext;
        private readonly ILoginRepository _loginRepository;
        private readonly string _apiKey;
        private readonly string _apiUrl;
        private readonly string _model;
        private HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private Dictionary<string, string> headers;
        private readonly CachingManagement _cachingManagement;

        public OpenAIService(
            IViralWaveContextProcedures viralWaveContext,
            ILoginRepository loginRepository,
            IConfiguration configuration, IMemoryCache memoryCache)
        {
            _viralWaveContext = viralWaveContext;
            _loginRepository = loginRepository;
            _configuration = configuration;
            _cachingManagement = new CachingManagement(memoryCache);

            _apiKey = configuration.GetValue<string>("GeminiAPIKey");
            _apiUrl = configuration.GetValue<string>("GeminiAPIURL");
            _model = configuration.GetValue<string>("GeminiModel");
            string referer = _configuration["Contrato:referer"];
            headers = new Dictionary<string, string>()
        {
                {"Content-Type","application/json"},
                {"Referer", referer },
                {"User-Agent","Mozilla" }
        };
        }


        public async Task<object> CreateAssistantWithFiles(string platformName, List<IFormFile> files)
        {
            var fileIds = new List<string>();



            var vectorStoreId = await CreateVectorStore(platformName);

            foreach (var fileId in fileIds)
            {
                await UploadFileToVectorStore(vectorStoreId, fileId);
            }

            var assistantId = await CreateAssistant(platformName, vectorStoreId);

            //foreach (var fileId in fileIds)
            //{
            //    await AttachFileToAssistant(assistantId, fileId);
            //}

            await _viralWaveContext.sp_InsertAssistantAsync(platformName, assistantId);

            return new
            {
                Platform = platformName,
                AssistantId = assistantId
            };
        }

        public async Task<string?> GetAssistantIdFromDb(string platformName)
        {
            var result = await _viralWaveContext.sp_GetAssistantIdByPlatformAsync(platformName);

            return result.FirstOrDefault()?.AssistantId;
        }

        public async Task<string> ChatWithAssistant(string platformName, string userMessage, string authHeader)
        {
            // Get Assistant ID from DB
            var assistantId = await GetAssistantIdFromDb(platformName);
            if (string.IsNullOrEmpty(assistantId))
                return $"❌ No assistant found for platform '{platformName}'";

            var userIdGUID = TokenService.GetUserId(authHeader);
            var userId = await _loginRepository.GetUserIdByGUIDAsync(userIdGUID);
            // Step 1: Create Thread
            var threadId = await GetOrCreateThread(userId, platformName);

            // Step 2: Send user message to thread
            await AddMessageToThread(threadId, userMessage);

            // Step 3: Run the assistant
            var runId = await RunAssistantOnThread(threadId, assistantId);

            // Step 4: Wait for completion
            await WaitForRunCompletion(threadId, runId);

            // Step 5: Get reply
            var reply = await GetLatestReplyFromThread(threadId);

            return reply;
        }

        #region Helper Methods

        private async Task<string> UploadPdfToOpenAI(Stream fileStream, string fileName)
        {
            var client = new RestClient(uploadUrl);
            var request = new RestRequest();
            request.AddHeader("Authorization", $"Bearer {apiKey}");

            request.AlwaysMultipartFormData = true;

            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();


            request.AddFile("file", fileBytes, fileName, contentType: "application/pdf");
            request.AddParameter("purpose", "assistants");

            var response = await client.PostAsync(request);
            dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(response.Content);
            return result.id;
        }

        private async Task<string> CreateAssistant(string platformName, string vectorStoreId)
        {
            var client = new RestClient(assistantUrl)
            {

            };
            var request = new RestRequest();
            request.AddHeader("Authorization", $"Bearer {apiKey}");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("OpenAI-Beta", "assistants=v2");
            request.Timeout = TimeSpan.FromSeconds(60);

            var body = new
            {
                name = $"{platformName}-assistant",
                instructions = $"You assist users with integration questions specific to {platformName}.",
                //model = "gpt-4-1106-preview",
                model = "gpt-3.5-turbo-1106",
                tools = new[] { new { type = "file_search" } },
                tool_resources = new
                {
                    file_search = new
                    {
                        vector_store_ids = new[] { vectorStoreId }
                    }
                }
            };

            var json = JsonConvert.SerializeObject(body, Formatting.Indented);
            Console.WriteLine("🧾 JSON Body:\n" + json);

            request.AddJsonBody(body);

            try
            {
                var response = await client.ExecutePostAsync(request);

                if (!response.IsSuccessful)
                {
                    Console.WriteLine("❌ Status Code: " + response.StatusCode);
                    Console.WriteLine("❌ Content: " + response.Content);
                    Console.WriteLine("🚨 OpenAI Error Content: " + response.Content);
                    var errorContent = response.Content ?? "No error content.";
                    throw new Exception($"OpenAI error: {response.StatusCode} - {errorContent}");
                }

                dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(response.Content);
                return result.id;
            }
            catch (Exception ex)
            {
                Console.WriteLine("🚨 Error creating assistant:");
                Console.WriteLine(ex.ToString());

                throw;
            }

        }

        private async Task<string> CreateThread()
        {
            var client = new RestClient("https://api.openai.com/v1/threads");
            var request = new RestRequest();
            request.AddHeader("Authorization", $"Bearer {apiKey}");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("OpenAI-Beta", "assistants=v2");

            var response = await client.ExecutePostAsync(request);
            dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(response.Content);
            return result.id;
        }

        private async Task AddMessageToThread(string threadId, string message)
        {
            var client = new RestClient($"https://api.openai.com/v1/threads/{threadId}/messages");
            var request = new RestRequest();
            request.AddHeader("Authorization", $"Bearer {apiKey}");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("OpenAI-Beta", "assistants=v2");

            var body = new { role = "user", content = message };
            request.AddJsonBody(body);

            await client.ExecutePostAsync(request);
        }

        private async Task<string> RunAssistantOnThread(string threadId, string assistantId)
        {
            var client = new RestClient($"https://api.openai.com/v1/threads/{threadId}/runs");
            var request = new RestRequest();
            request.AddHeader("Authorization", $"Bearer {apiKey}");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("OpenAI-Beta", "assistants=v2");

            var body = new { assistant_id = assistantId };
            request.AddJsonBody(body);

            var response = await client.ExecutePostAsync(request);
            dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(response.Content);
            return result.id;
        }

        private async Task WaitForRunCompletion(string threadId, string runId)
        {
            var client = new RestClient($"https://api.openai.com/v1/threads/{threadId}/runs/{runId}");
            var request = new RestRequest();
            request.AddHeader("Authorization", $"Bearer {apiKey}");
            request.AddHeader("OpenAI-Beta", "assistants=v2");

            string status = "queued";
            while (status != "completed" && status != "failed" && status != "cancelled")
            {
                await Task.Delay(1000);
                var response = await client.GetAsync(request);
                dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(response.Content);
                status = result.status;
            }
        }

        private async Task<string> GetLatestReplyFromThread(string threadId)
        {
            var client = new RestClient($"https://api.openai.com/v1/threads/{threadId}/messages");
            var request = new RestRequest();
            request.AddHeader("Authorization", $"Bearer {apiKey}");
            request.AddHeader("OpenAI-Beta", "assistants=v2");

            var response = await client.GetAsync(request);
            dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(response.Content);
            var messages = result.data;

            foreach (var message in messages)
            {
                if (message.role == "assistant")
                    return message.content[0].text.value;
            }

            return "⚠️ Assistant did not reply.";
        }

        private async Task<string> GetOrCreateThread(int userId, string platformName)
        {
            var thread = await _viralWaveContext.sp_GetThreadByUserAndPlatformAsyncAsync(userId, platformName);

            if (thread != null && thread.Any())
                return thread.FirstOrDefault()?.ThreadId;

            // Create new thread
            var newThreadId = await CreateThread();

            // Save it to DB
            await _viralWaveContext.sp_InsertUserThreadAsyncAsync(userId, platformName, newThreadId);

            return newThreadId;
        }

        private async Task AttachFileToAssistant(string assistantId, string fileId)
        {
            var client = new RestClient($"https://api.openai.com/v1/assistants/{assistantId}/files");
            var request = new RestRequest();
            request.AddHeader("Authorization", $"Bearer {apiKey}");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("OpenAI-Beta", "assistants=v2");

            var body = new { file_id = fileId };
            request.AddJsonBody(body);

            var response = await client.ExecutePostAsync(request);

            if (!response.IsSuccessful)
            {
                Console.WriteLine("❌ Error attaching file: " + response.Content);
                throw new Exception("Failed to attach file to assistant.");
            }
        }

        private async Task<string> CreateVectorStore(string platformName)
        {
            var client = new RestClient("https://api.openai.com/v1/vector_stores");
            var request = new RestRequest();
            request.AddHeader("Authorization", $"Bearer {apiKey}");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("OpenAI-Beta", "assistants=v2");

            var body = new { name = $"{platformName}-store" };
            request.AddJsonBody(body);

            var response = await client.ExecutePostAsync(request);

            if (!response.IsSuccessful)
                throw new Exception("❌ Failed to create vector store: " + response.Content);

            dynamic result = JsonConvert.DeserializeObject(response.Content);
            return result.id;
        }

        private async Task UploadFileToVectorStore(string vectorStoreId, string fileId)
        {
            var client = new RestClient($"https://api.openai.com/v1/vector_stores/{vectorStoreId}/files");
            var request = new RestRequest();
            request.AddHeader("Authorization", $"Bearer {apiKey}");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("OpenAI-Beta", "assistants=v2");

            var body = new { file_id = fileId };
            request.AddJsonBody(body);

            var response = await client.ExecutePostAsync(request);

            if (!response.IsSuccessful)
                throw new Exception("❌ Failed to attach file to vector store: " + response.Content);
        }
        public async Task<string> RequestGeminiAI(RequestGeminiAIDto model)
        {
            var contractDataCacheKey = $"{model.contents?.First().PartyName}_{model.contents?.First().StartDate}_{model.contents?.First().EndDate}";
            var geminiResponse = "";
            //Check if the data is already in the cache
            var cachedData = await _cachingManagement.GetCachedDataAsync(contractDataCacheKey);

            if (cachedData != null)
            {
                //If the data is in the cache, use it to generate the prompt
                model.contents[0].parts[0].text =
                    ConstantVariables.aiPromptGenerator(cachedData, model.contents?[0]?.parts?[0].text ?? string.Empty);
            }
            else
            {
                //If the data is not in the cache, send a request to the Contrato API
                //Send request to Contrato API for Document Data:
                var firstContent = model.contents.First();

                var partyName = firstContent.PartyName;
                var startDate = firstContent.StartDate == default ? (DateTime?)null : firstContent.StartDate;
                var endDate = firstContent.EndDate == default ? (DateTime?)null : firstContent.EndDate;

                var queryParams = new List<string>();
                if (!string.IsNullOrEmpty(partyName))
                    queryParams.Add($"PartyName={Uri.EscapeDataString(partyName)}");
                if (startDate.HasValue)
                    queryParams.Add($"StartDate={startDate.Value:O}");
                if (endDate.HasValue)
                    queryParams.Add($"EndDate={endDate.Value:O}");

                string queryString = string.Join("&", queryParams);

                var contractBaseUrl = _configuration["Contrato:ApiUrl"];
                var fullUrl = $"{contractBaseUrl}DocumentInfo/GetDocumentsInfoByParty?{queryString}";

                var contratoDocumentResponse = await MakeContratoRequest(HttpMethod.Get, fullUrl);

                if (string.IsNullOrEmpty(contratoDocumentResponse))
                {
                    throw new Exception("Failed to get documents from Contrato API.");
                }
                await _cachingManagement.CacheDataAsync(contractDataCacheKey, contratoDocumentResponse, TimeSpan.FromMinutes(10));

                //passing the response from contrato to generate the prompt:
                model.contents[0].parts[0].text =
                    ConstantVariables.aiPromptGenerator(contratoDocumentResponse, model.contents?[0]?.parts?[0].text ?? string.Empty);

            }
            //Send request to Gemini API for contract data:
            geminiResponse = await MakeGeminiRequest(HttpMethod.Post, model);
            if (string.IsNullOrEmpty(geminiResponse))
            {
                throw new Exception("Failed to get response from Gemini API.");
            }
            return geminiResponse;

        }

        private async Task<string?> MakeGeminiRequest(HttpMethod httpMethod, RequestGeminiAIDto model)
        {
            _httpClient = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_apiUrl}/{_model}:generateContent?key={_apiKey}");
            //Remove the party name, start date, and end date from the request body
            var geminiAIModel = new
            {
                contents = model.contents?.Select(c => new
                {
                    parts = new[]
                    {
                        new
                        {
                            text = c.parts?.Select(p => p.text).FirstOrDefault(),
                        }
                    }
                }).ToList()
            };

            request.Content = new StringContent(JsonConvert.SerializeObject(geminiAIModel), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            var jsonDocument = JsonDocument.Parse(jsonString);
            var data =
                jsonDocument.RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0]
                    .GetProperty("text").GetString();
            if (string.IsNullOrEmpty(data?.ToString()))
            {
                throw new Exception("Failed to get response from Gemini API.");
            }
            return data;
        }

        #endregion

        private async Task<string?> MakeContratoRequest(HttpMethod httpMethod, string baseUrl, StringContent content = null)
        {
            var accessToken = await GetAccessToken();
            if (accessToken is null)
            {
                return null;
            }

            AddingAuthorization("Authorization", $"Bearer {accessToken.accessToken}");
            try
            {

                HttpResponseMessage response;
                if (content is null)
                    response = await HttpClientHelper.SendAsync(httpMethod, baseUrl, headers);
                else
                    response = await HttpClientHelper.SendAsync(httpMethod, baseUrl, content, headers);

                if (response.IsSuccessStatusCode)
                {
                    var encodingResponse = await response.Content.ReadAsStringAsync();
                    return encodingResponse;
                }
                else
                {

                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void AddingAuthorization(string key, string value)
        {
            if (!headers.ContainsKey(key))
                headers.Add(key, value);
        }

        private async Task<AccessToken?> GetAccessToken()
        {
            var baseUrl = _configuration["Contrato:ApiUrl"] + "Login/AddUser";
            var retryCount = 3;
            AccessToken accessToken = new AccessToken
            {
                userName = _configuration["Contrato:UserName"],
                pass = _configuration["Contrato:Password"],
                accessToken = _configuration["Contrato:VirtualToken"],
                isAuthByMicrosoft = true,
            };

            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(accessToken), Encoding.UTF8, "application/json");



            for (int retry = 0; retry < retryCount; retry++)
            {

                try
                {
                    HttpResponseMessage response = await HttpClientHelper.SendAsync(HttpMethod.Post, baseUrl, content, headers);

                    if (response.IsSuccessStatusCode)
                    {
                        var encodedResponse = await response.Content.ReadAsStringAsync();
                        var res = JsonConvert.DeserializeObject<AccessToken>(encodedResponse);
                        return res;
                    }
                    else
                    {
                    }
                }
                catch (Exception)
                {
                }
                // If it's not the last retry, wait before trying again
                if (retry < retryCount - 1)
                {
                    await Task.Delay(1000); // Delay for 1 second before retrying
                }
            }
            // If all retries failed, return null or throw an exception
            return null;
        }
    }

}


