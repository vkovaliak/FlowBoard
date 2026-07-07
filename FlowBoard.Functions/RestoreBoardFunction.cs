using System.Text.Json;
using FlowBoard.Domain.Constants;
using FlowBoard_Functions.Models;
using FlowBoard_Functions.Services;
using Microsoft.Azure.Functions.Worker;

namespace FlowBoard_Functions;

public class RestoreBoardFunction
{
    private readonly IRestoreBoardProcessor _processor;

    public RestoreBoardFunction(IRestoreBoardProcessor processor)
    {
        _processor = processor;
    }

    [Function(nameof(RestoreBoardFunction))]
    public async Task Run(
        [ServiceBusTrigger(
            ArchiveBoardConstants.BoardRestoreQueue,
            Connection = ArchiveBoardConstants.BoardArchiveConnection)]
        string messageBody)
    {
        var message = JsonSerializer.Deserialize<BoardArchiveMessage>(
            messageBody);

        if (message is null)
        {
            return;
        }

        await _processor.ProcessAsync(message.BoardId);
    }
}