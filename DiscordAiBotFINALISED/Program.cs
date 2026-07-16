using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace DiscordAiBotFINALISED
{
    internal class Program
    {
        private readonly DiscordSocketClient _client;
        private readonly HttpClient _httpClient;

        private const string discordToken = "YOUR_DISCORD_BOT_TOKEN"; // Replace with your Discord bot token
        private const string geminiApiKey = "YOUR_GEMINI_API_KEY";  // Replace with your Gemini API key

        public Program()
        {
            this._httpClient = new HttpClient();

            var config = new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
            };

            this._client = new DiscordSocketClient(config);
            this._client.MessageReceived += MessageHandler;
        }

        public async Task StartBotAsync()
        {
            this._client.Log += LogAsync;
            await this._client.LoginAsync(TokenType.Bot, discordToken);
            await this._client.StartAsync();
            await Task.Delay(-1);
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private async Task MessageHandler(SocketMessage message)
        {
            if (message.Author.IsBot) { return; }

            bool mentioned = message.MentionedUsers.Any(u => u.Id == this._client.CurrentUser.Id);
            bool isPrivate = message.Channel is IPrivateChannel;

            if (isPrivate || mentioned)
            {
                string userPrompt = message.Content.Replace($"<@{this._client.CurrentUser.Id}>", "").Trim();

                if (string.IsNullOrWhiteSpace(userPrompt))
                {
                    await ReplyAsync(message, "Hello! I'm your AI assistant. Ask me anything!"); //Enter your custom idle message here
                    return;
                }

                using (message.Channel.EnterTypingState())
                {
                    string aiResponse = await GetAiResponseAsync(userPrompt);

                    if (aiResponse.Length > 2000)
                    {
                        aiResponse = aiResponse.Substring(0, 1990) + "...";
                    }

                    await ReplyAsync(message, aiResponse);
                }
            }
        }

        private const string systemPrompt = "YOUR_AI_PROMPT";  // Here you can set the behaviour for your AI (optional)
        private async Task<string> GetAiResponseAsync(string userPrompt)
        {
            string url = $"YOUR_GEMINI_API_URL";     // Replace with your Gemini API URL

            var requestBody = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = systemPrompt } } },
                    new { parts = new[] { new { text = userPrompt } } }
                }
            };

            int maxRetries = 3;
            int delayMs = 2000;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    string jsonPayload = JsonConvert.SerializeObject(requestBody);
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    var response = await _httpClient.PostAsync(url, content);

                    if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable ||
                        (int)response.StatusCode == 429)
                    {
                        Console.WriteLine($"[ATTEMPT {attempt} OF {maxRetries}]: ERROR (Code {(int)response.StatusCode}). RETRY {delayMs / 1000} sec...");

                        await Task.Delay(delayMs);
                        delayMs *= 2;
                        continue;
                    }

                    if (!response.IsSuccessStatusCode)
                    {
                        string errorResponse = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"[GOOGLE ERROR]: Code {(int)response.StatusCode}. Reply: {errorResponse}");
                        return "Couldn't get a response from the AI.";
                    }

                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    dynamic result = JsonConvert.DeserializeObject(jsonResponse);
                    return result.candidates[0].content.parts[0].text;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR ON ATTEMPT {attempt}]: {ex.Message}");
                    if (attempt == maxRetries) return "Error occurred while fetching AI response.";

                    await Task.Delay(delayMs);
                    delayMs *= 2;
                }
            }

            return "AI servers are currently overloaded or you're out of tokens.";
        }

        private async Task ReplyAsync(SocketMessage message, string aiResponse) =>
            await message.Channel.SendMessageAsync(aiResponse);

        static void Main(string[] args)
        {
            var program = new Program();
            program.StartBotAsync().GetAwaiter().GetResult();
        }
    }
}