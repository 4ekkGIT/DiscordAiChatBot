# 🤖 Discord AI Bot (C# + Discord.NET + Google Gemini)

A lightweight AI-powered Discord bot built with **C#**, **Discord.NET**, and the **Google Gemini API**. The bot responds whenever it is mentioned in a server or when a user sends it a direct message, making it easy to integrate an AI assistant into any Discord community.

## ✨ Features

* 💬 Responds when mentioned in a Discord server
* 📩 Supports private (DM) conversations
* 🧠 Powered by Google's Gemini AI
* 🔄 Automatic retry system with exponential backoff for API rate limits and temporary outages
* 📝 Customizable system prompt for AI personality and behavior

## 🛠️ Technologies Used

* **C#**
* **.NET**
* **Discord.NET**
* **Google Gemini API**
* **Newtonsoft.Json**
* **HttpClient**

## 📂 Project Structure

```
DiscordAiBot/
│
├── Program.cs          # Main application logic
├── README.md           # Project documentation
└── ...
```

## ⚙️ Configuration

Before running the bot, replace the placeholder values in `Program.cs`:

```csharp
private const string discordToken = "YOUR_DISCORD_BOT_TOKEN";
private const string geminiApiKey = "YOUR_GEMINI_API_KEY";
private const string systemPrompt = "YOUR_AI_PROMPT";
```

Also replace:

```csharp
string url = "YOUR_GEMINI_API_URL";
```

with the correct Gemini API endpoint.

## 🚀 Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/yourusername/DiscordAiBot.git
cd DiscordAiBot
```

### 2. Install dependencies

Restore the required NuGet packages.

### 3. Configure the bot

Insert your:

* Discord Bot Token
* Google Gemini API Key
* Gemini API URL
* Optional system prompt

### 4. Run the project

```bash
dotnet run
```

The bot will connect to Discord and start listening for messages.

## 💡 How It Works

1. User mentions the bot or sends it a DM.
2. The bot removes its mention from the message.
3. The prompt is sent to the Gemini API.
4. The AI generates a response.
5. The bot sends the response back to Discord.

If Gemini temporarily returns a **429 (Rate Limited)** or **503 (Service Unavailable)** error, the bot automatically retries using exponential backoff.

## 📦 Dependencies

* Discord.NET
* Newtonsoft.Json
* .NET Runtime

## 🔮 Possible Future Improvements

* Conversation memory
* Per-user chat history
* Slash commands
* Image generation support
* Streaming responses
* Multiple AI provider support (OpenAI, Claude, Ollama, etc.)
* Configuration through JSON instead of hardcoded constants
* Docker support
* Logging system
* Admin commands
* Personality presets

## 📄 License

This project is open-source and available under the MIT License.

---

### Disclaimer

This project is an educational demonstration of integrating the Discord API with Google's Gemini AI. API usage may incur costs depending on your Gemini API plan.
