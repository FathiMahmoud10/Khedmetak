using Azure;
using Khedmetak.AI.Configuration;
using Khedmetak.AI.DTOs.ChatSessionDTO;
using Khedmetak.AI.Services.Abstraction;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.Abstraction.UnitOfWork;
using Khedmetak.DAL.Repo.shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ChatMessage = OpenAI.Chat.ChatMessage;

namespace Khedmetak.AI.Services.Implementation
{
    public class AIChatService : IAIChatService
    {
        private readonly ChatClient chat;

        public AIChatService([FromKeyedServices("github")] OpenAIClient githubClient, IOptions<AISettings> settings)
        {
            chat = githubClient.GetChatClient(settings.Value.Model);

        }

        public async Task<string> AskAsync(string newUserMessage)
        {
            ChatCompletion completion = await chat.CompleteChatAsync(
                newUserMessage);

            return completion.Content[0].Text;
        }

        public async Task<string> AskAsync( string newUserMessage, ChatSessionDTO chatSessionDto)
        {
            // chatmessage builtin Open AI not our Entity database
            List<ChatMessage> messages = new();

            messages.Add(ChatMessage.CreateSystemMessage(
                """
                You are an Egyptian Government Services Assistant.

                Always answer in Egyptian Arabic.

                Formatting rules:
                - Use Markdown.
                - Use headings (##).
                - Use bullet lists (-).
                - Use numbered lists (1. 2. 3.).
                - Never output JSON.
                - Never output escaped characters such as \n or \r\n.
                - Keep answers concise and structured.

                Response Template:

                # {Service Name}

                ## 📋 Required Documents
                - Document 1
                - Document 2

                ## 📝 Steps
                1. Step 1
                2. Step 2

                ## 💰 Fees
                - Fee information
                - If unavailable, write: "غير متوفر حالياً"

                ## ⏳ Processing Time
                - Processing time
                - If unavailable, write: "غير متوفر حالياً"


                """)
            );
         

            // Previous messages
            if (chatSessionDto?.ChatSession_ChatHistory != null)
            {
                foreach (var msg in chatSessionDto.ChatSession_ChatHistory)
                {
                    if (msg.Role.Equals("user", StringComparison.OrdinalIgnoreCase))
                    {
                        messages.Add(
                            ChatMessage.CreateUserMessage(msg.Content));
                    }
                    else if (msg.Role.Equals("assistant", StringComparison.OrdinalIgnoreCase))
                    {
                        messages.Add(
                            ChatMessage.CreateAssistantMessage(msg.Content));
                    }
                    else
                    {
                        messages.Add(
                            ChatMessage.CreateSystemMessage(msg.Content));
                    }
                }
            }

            // Current user message
            messages.Add(
                ChatMessage.CreateUserMessage(newUserMessage));

            // send request to aI model and wait to response
            ChatCompletion completion = await chat.CompleteChatAsync(messages);

            return completion.Content[0].Text;
        }



    public async Task<string> AskWithContextAsync(string userQuestion,string context,ChatSessionDTO? chatSessionDto)
    {
        List<ChatMessage> messages = new();

        messages.Add(ChatMessage.CreateSystemMessage(
            """
        You are Khedmetak AI Assistant.

        You must answer using ONLY the provided context.
        If the answer is not in the context, reply exactly:
        "I couldn't find this information 

        in the knowledge base."

        Important Output Rules:
        - Always respond in valid JSON format with the key "message".
        - The "message" field must contain **plain text with actual newlines**, NOT escaped \\n or \n.
        - Use real line breaks inside the message value.
        - Use markdown formatting (**, numbered lists, bullets) for readability.
        - Do not escape any characters inside the "message" field except when absolutely necessary.

        Correct Example:
        {
            "message": "لإجديد رخصة السيارة اتبع الخطوات التالية:

        1. **فحص المستندات**:
            يجب فحص الرخصة القديمة والمستندات المطلوبة.

        2. **إحضار المستندات**:
            أحضر الرخصة القديمة وكافة الأوراق.

        مدة التنفيذ: 1 يوم
        الرسوم: 500.00 جنيه"
        }
        """));

        messages.Add(ChatMessage.CreateSystemMessage(
            $"""
                 Context:
                    {context}
             """
        ));

            if (chatSessionDto?.ChatSession_ChatHistory != null)
            {
                foreach (var msg in chatSessionDto.ChatSession_ChatHistory.TakeLast(10))
                {
                    if (msg.Role.Equals("user", StringComparison.OrdinalIgnoreCase))
                    {
                        messages.Add(ChatMessage.CreateUserMessage(msg.Content));
                    }
                    else if (msg.Role.Equals("assistant", StringComparison.OrdinalIgnoreCase))
                    {
                        messages.Add(ChatMessage.CreateAssistantMessage(msg.Content));
                    }
                }
            }

            messages.Add(ChatMessage.CreateUserMessage(userQuestion));

            ChatCompletion completion = await chat.CompleteChatAsync(messages);
            var response = completion.Content[0].Text;
        
            return response;
        }


    public async Task<string> RewriteQuestionAsync(string userQuestion,ChatSessionDTO? chatSessionDto)
        {
            List<ChatMessage> messages = new();

            messages.Add(ChatMessage.CreateSystemMessage(
                """
        Rewrite the user's question as a standalone question.

        Use the conversation history for context.

        Return ONLY the rewritten question.
        """
            ));

            if (chatSessionDto?.ChatSession_ChatHistory != null)
            {
                foreach (var msg in chatSessionDto.ChatSession_ChatHistory.TakeLast(10))
                {
                    if (msg.Role.Equals("user", StringComparison.OrdinalIgnoreCase))
                    {
                        messages.Add(ChatMessage.CreateUserMessage(msg.Content));
                    }
                    else if (msg.Role.Equals("assistant", StringComparison.OrdinalIgnoreCase))
                    {
                        messages.Add(ChatMessage.CreateAssistantMessage(msg.Content));
                    }
                }
            }

            messages.Add(ChatMessage.CreateUserMessage(userQuestion));

            ChatCompletion completion =
                await chat.CompleteChatAsync(messages);

            return completion.Content[0].Text;
        }
        // ------------------------ Response API ------------------

        //private readonly HttpClient _httpClient;
        //private readonly AISettings _settings;

        //public AIChatService(HttpClient httpClient,IOptions<AISettings> options)
        //{
        //    _httpClient = httpClient;
        //    _settings = options.Value;
        //}

        //public async Task<string> AskAsync(string newUserMessage)
        //{
        //    var requestBody = new
        //    {
        //        model = _settings.Model,
        //        input = newUserMessage
        //    };

        //    using var request = new HttpRequestMessage(
        //        HttpMethod.Post,
        //        "responses");

        //    request.Headers.Authorization =
        //        new AuthenticationHeaderValue(
        //            "Bearer",
        //            _settings.ApiKey);

        //    request.Content = new StringContent(
        //        JsonSerializer.Serialize(requestBody),
        //        Encoding.UTF8,
        //        "application/json");

        //    var response = await _httpClient.SendAsync(request);

        //    response.EnsureSuccessStatusCode();

        //    var json = await response.Content.ReadAsStringAsync();

        //    using var document = JsonDocument.Parse(json);

        //    return document.RootElement
        //        .GetProperty("output")[0]
        //        .GetProperty("content")[0]
        //        .GetProperty("text")
        //        .GetString() ?? string.Empty;
        //}


        //public async Task<string> AskAsync( string newUserMessage, ChatSessionDTO chatSessionDto)
        //{
        //    // Build conversation history
        //    var inputMessages = new List<object>();

        //    // 1. Add previous messages from session
        //    if (chatSessionDto?.ChatSession_ChatHistory != null)
        //    {
        //        foreach (var msg in chatSessionDto.ChatSession_ChatHistory)
        //        {
        //            inputMessages.Add(new
        //            {
        //                role = msg.Role,   // "user" or "assistant"
        //                content = msg.Content
        //            });
        //        }
        //    }

        //    // 2. Add the new user message
        //    inputMessages.Add(new
        //    {
        //        role = "user",
        //        content = newUserMessage
        //    });

        //    // 3. Build request
        //    var requestBody = new
        //    {
        //        model = _settings.Model,
        //        input = inputMessages,
        //        max_output_tokens = _settings.MaxToken
        //    };

        //    using var request = new HttpRequestMessage(
        //        HttpMethod.Post,
        //        "responses");

        //    request.Headers.Authorization =
        //        new AuthenticationHeaderValue("Bearer", _settings.ApiKey);

        //    request.Content = new StringContent(
        //        JsonSerializer.Serialize(requestBody),
        //        Encoding.UTF8,
        //        "application/json");

        //    var response = await _httpClient.SendAsync(request);

        //    response.EnsureSuccessStatusCode();

        //    var json = await response.Content.ReadAsStringAsync();

        //    using var document = JsonDocument.Parse(json);

        //    return document.RootElement
        //        .GetProperty("output")[0]
        //        .GetProperty("content")[0]
        //        .GetProperty("text")
        //        .GetString() ?? string.Empty;
        //}










    }
    
}
