using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using System.ClientModel;

// where to get credentials from
// get credentials from user secrets
IConfigurationRoot config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();
// which credentials to use
var credential = new ApiKeyCredential(config["GitHubModels:Token"] ?? throw new InvalidOperationException());
// which endpoint to use for AI
var options = new OpenAIClientOptions()
{
    Endpoint = new Uri("https://models.github.ai/inference")
};
// create a chat client
IChatClient client = 
    new ChatClientBuilder(new OpenAIClient(credential, options).GetChatClient("openai/gpt-4.1-mini").AsIChatClient())
    .UseFunctionInvocation() // forces the model to use function invocation
    .Build();


var chatOptions = new ChatOptions()
{
    Tools = [AIFunctionFactory.Create((string location, string unit) => 
    {
        // Here you would call a weather API to get the current weather for the given location and unit.
        var temperature = Random.Shared.Next(30, 105); // Simulated temperature
        var conditions = Random.Shared.Next(0, 1) == 0 ? "sunny" : "cloudy"; // Simulated conditions

        return $"The current weather is {temperature} Fahrenheit degrees and {conditions}.";
    },
    "get_current_weather",
    "Get the current weather in a given location")]
};

List<ChatMessage> chatHistory = [new(ChatRole.System, """
    You are a hiking enthusiast who helps people discover fun hikes in their area.
    You are upbeat and friendly.
    """)];

// Weather conversation relevant to the registered function
chatHistory.Add(new(ChatRole.User, """
    I live in Puyallup, WA and I'm looking for a moderate intensity hike. What's the current weather like?
    """));

Console.WriteLine($"{chatHistory.Last().Role} >>> {chatHistory.Last()}");

ChatResponse response = await client.GetResponseAsync(chatHistory, chatOptions);

chatHistory.Add(new(ChatRole.Assistant, response.Text));

Console.WriteLine($"{chatHistory.Last().Role} >>> {chatHistory.Last()}");

