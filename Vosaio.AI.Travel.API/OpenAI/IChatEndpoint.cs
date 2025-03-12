using OpenAI.Chat;

namespace Vosaio.AI.Travel.API.OpenAI;

public interface IChatEndpoint
{
    Task<ChatResponse> GetCompletionAsync(ChatRequest request);
}
