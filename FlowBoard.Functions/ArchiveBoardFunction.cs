using System.Text.Json;
using FlowBoard_Functions.Constants;
using FlowBoard_Functions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FlowBoard_Functions;

public class ArchiveBoardFunction
{
    private readonly ILogger<ArchiveBoardFunction> _logger;

    public ArchiveBoardFunction(ILogger<ArchiveBoardFunction> logger)
    {
        _logger = logger;
    }

    [Function(nameof(ArchiveBoardFunction))]
    public void Run(
        [ServiceBusTrigger(
            ArchiveBoardConstants.BoardArchiveQueue, 
            Connection = ArchiveBoardConstants.BoardArchiveConnection)]
        string messageBody)
    {
        _logger.LogInformation(
            "ArchiveBoardFunction: received message {Body}", messageBody);

        var message = JsonSerializer.Deserialize<BoardArchiveMessage>(messageBody);

        if (message is null)
        {
            _logger.LogWarning("ArchiveBoardFunction: failed to deserialize message");
            return;
        }

        _logger.LogInformation(
            "ArchiveBoardFunction: processing board {BoardId}", message.BoardId);
    }
}