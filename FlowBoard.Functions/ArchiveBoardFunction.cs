using System.Text.Json;
using FlowBoard.Domain.Constants;
using FlowBoard_Functions.Models;
using FlowBoard_Functions.Services;
using Microsoft.Azure.Functions.Worker;

namespace FlowBoard_Functions;

public class ArchiveBoardFunction
{
    private readonly IArchiveBoardProcessor _processor;

    public ArchiveBoardFunction(IArchiveBoardProcessor processor)
    {
        _processor = processor;
    }

    [Function(nameof(ArchiveBoardFunction))]
    public void Run(
        [ServiceBusTrigger(
            ArchiveBoardConstants.BoardArchiveQueue, 
            Connection = ArchiveBoardConstants.BoardArchiveConnection)]
        string messageBody)
    {
        var message = JsonSerializer.Deserialize<BoardArchiveMessage>(messageBody);

        if (message is null)
        {
            return;
        }

        _processor.ProcessAsync(message.BoardId);
    }
}