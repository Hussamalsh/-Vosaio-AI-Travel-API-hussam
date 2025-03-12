using OpenAI;

namespace Vosaio.AI.Travel.API.OpenAI;

public class OpenAIClientAdapter : IOpenAIClient
{
    private readonly OpenAIClient _client;

    public OpenAIClientAdapter(OpenAIClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public IChatEndpoint ChatEndpoint => new ChatEndpointAdapter(_client.ChatEndpoint);

    public void Dispose()
    {
        _client.Dispose();
    }
}
