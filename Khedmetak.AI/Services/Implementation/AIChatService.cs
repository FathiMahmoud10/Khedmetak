using Azure;
using Khedmetak.AI.Configuration;
using Khedmetak.AI.DTOs.ChatSessionDTO;
using Khedmetak.AI.Services.Abstraction;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.Abstraction.UnitOfWork;
using Khedmetak.DAL.Repo.shared;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Khedmetak.AI.Services.Implementation
{
    public class AIChatService : IAIChatService
    {

        private readonly HttpClient _httpClient;
        private readonly AISettings _settings;

        public AIChatService(HttpClient httpClient,IOptions<AISettings> options)
        {
            _httpClient = httpClient;
            _settings = options.Value;
        }

        public async Task<string> AskAsync(string newUserMessage)
        {
            var requestBody = new
            {
                model = _settings.Model,
                input = newUserMessage
            };

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "responses");

            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    _settings.ApiKey);

            request.Content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(json);

            return document.RootElement
                .GetProperty("output")[0]
                .GetProperty("content")[0]
                .GetProperty("text")
                .GetString() ?? string.Empty;
        }

       
        public async Task<string> AskAsync( string newUserMessage, ChatSessionDTO chatSessionDto)
        {
            // Build conversation history
            var inputMessages = new List<object>();

            // 1. Add previous messages from session
            if (chatSessionDto?.ChatSession_ChatHistory != null)
            {
                foreach (var msg in chatSessionDto.ChatSession_ChatHistory)
                {
                    inputMessages.Add(new
                    {
                        role = msg.Role,   // "user" or "assistant"
                        content = msg.Content
                    });
                }
            }

            // 2. Add the new user message
            inputMessages.Add(new
            {
                role = "user",
                content = newUserMessage
            });

            // 3. Build request
            var requestBody = new
            {
                model = _settings.Model,
                input = inputMessages,
                max_output_tokens = _settings.MaxToken
            };

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "responses");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", _settings.ApiKey);

            request.Content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(json);

            return document.RootElement
                .GetProperty("output")[0]
                .GetProperty("content")[0]
                .GetProperty("text")
                .GetString() ?? string.Empty;
        }










    }
    
}
