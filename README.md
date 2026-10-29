# Generative AI with .NET: where I started

This repo is my first hands-on exercise with generative AI in .NET. The projects are small on purpose: each one tries out a single idea, starting from "send a prompt, get a reply" and working up to a chat app that answers questions about your own documents (RAG), which will be my next project.

I'm keeping it as a record of where I started. The code is learning code, not production code.

## The learning path

The projects are listed in the order I built them. Each one builds on the one before.

| # | Project | What it tries | Runs on |
|---|---------|---------------|---------|
| 1 | [TextCompletion](TextCompletion/) | Basic prompts, streaming, classification, summarization, sentiment analysis, structured output (JSON into a C# class), chat with memory | GitHub Models (cloud) |
| 2 | [TextCompletion_Ollama](TextCompletion_Ollama/) | The same demos, running on a local model instead of the cloud | Ollama (local) |
| 3 | [Embeddings](Embeddings/) | Turning text into vectors and comparing meaning with cosine similarity ("cat" vs "kitten" vs "dog") | GitHub Models |
| 4 | [VectorSearch](VectorSearch/) | Storing movie descriptions as vectors and searching by meaning ("family friendly movie") | GitHub Models |
| 5 | [VectorSearch-Ollama](VectorSearch-Ollama/) | The same vector search with a local embedding model | Ollama |
| 6 | [FunctionCall](FunctionCall/) | Letting the model call a C# function (a fake weather lookup) | GitHub Models |

### What I learned along the way

- **`IChatClient` makes switching providers easy.** Moving from GitHub Models to Ollama only changes the line that creates the client. The rest of the code stays the same ([Microsoft.Extensions.AI](https://learn.microsoft.com/dotnet/ai/microsoft-extensions-ai)).
- **A model has no memory.** "Chat with memory" works by sending the whole conversation history with every request.
- **Embeddings capture meaning, not spelling.** "cat" and "kitten" come out closer than "cat" and "dog".
- **Vector search + a chat model = RAG.** Find the relevant chunks of your documents, give them to the model, and it can answer about data it was never trained on.
- **Keep secrets out of code.** API tokens are stored with .NET user secrets, which live outside the project folder and never end up in git.

## Try it yourself

### Requirements

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- **For the GitHub Models projects:** a GitHub account and a [personal access token](https://github.com/settings/personal-access-tokens) with the **Models: read** permission. GitHub Models has a free tier with rate limits.
- **For the Ollama projects:** [Ollama](https://ollama.com) installed and running, plus these models:
  ```sh
  ollama pull llama3.2
  ollama pull all-minilm
  ```

### 1. Clone the repo

```sh
git clone https://github.com/<your-username>/genai-dotnet.git
cd genai-dotnet
```

### 2. Add your GitHub token (GitHub Models projects only)

Each cloud project reads the token from user secrets under the key `GitHubModels:Token`. Set it once for each project you want to run:

```sh
dotnet user-secrets set "GitHubModels:Token" "<your-token>" --project TextCompletion
dotnet user-secrets set "GitHubModels:Token" "<your-token>" --project Embeddings
dotnet user-secrets set "GitHubModels:Token" "<your-token>" --project VectorSearch
dotnet user-secrets set "GitHubModels:Token" "<your-token>" --project FunctionCall
```

The token is saved on your machine (in `%APPDATA%\Microsoft\UserSecrets` on Windows, or `~/.microsoft/usersecrets` on macOS/Linux), not in the repo.

### 3. Run a project

```sh
dotnet run --project TextCompletion
```


### Switching demos on and off

`TextCompletion` and `TextCompletion_Ollama` contain several demos in one `Program.cs`, each in its own `#region` block. Most are commented out with `/* ... */`, so only one runs at a time. To try a different demo, comment out the active one and uncomment the one you want.

## Tech used

- [Microsoft.Extensions.AI](https://learn.microsoft.com/dotnet/ai/microsoft-extensions-ai): one set of abstractions (`IChatClient`, `IEmbeddingGenerator`) for any AI provider
- [GitHub Models](https://github.com/marketplace/models): hosted models (`openai/gpt-4.1-mini`, `openai/text-embedding-3-small`) through an OpenAI-compatible endpoint
- [Ollama](https://ollama.com) with [OllamaSharp](https://github.com/awaescher/OllamaSharp): local models (`llama3.2`, `all-minilm`)
- Microsoft.Extensions.VectorData with an in-memory vector store
