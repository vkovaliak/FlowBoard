namespace FlowBoard.Application.Abstractions;

public interface ISearchContextProvider
{
    Task<string> GetContextAsync(string query);
}