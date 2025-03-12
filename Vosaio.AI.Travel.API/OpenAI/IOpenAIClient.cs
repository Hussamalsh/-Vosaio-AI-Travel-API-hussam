
namespace Vosaio.AI.Travel.API.OpenAI;

public interface IOpenAIClient : IDisposable
{
    IChatEndpoint ChatEndpoint { get; }
}