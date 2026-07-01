namespace FlowBoard.Infrastructure.Chat.Prompts;

internal static class ChatPrompts
{
    public const string SystemPromptTemplate =
        """
        You are a helpful AI assistant for the FlowBoard company.
        Answer user questions using ONLY the provided context from the company's documents.
        If the answer cannot be found in the context, politely inform the user
        that you do not possess this information. Do not make up answers.

        CONTEXT:
        {0}
        """;

    public static string BuildSystemPrompt(string context)
        => string.Format(SystemPromptTemplate, context);
}