using OpenAI.Chat;

namespace Vosaio.AI.Travel.API.OpenAI;

public class ChatEndpointAdapter : IChatEndpoint
{
    private readonly ChatEndpoint _chatEndpoint;

    public ChatEndpointAdapter(ChatEndpoint chatEndpoint)
    {
        _chatEndpoint = chatEndpoint;
    }

    public Task<ChatResponse> GetCompletionAsync(ChatRequest request)
    {
        return _chatEndpoint.GetCompletionAsync(request);
    }
}