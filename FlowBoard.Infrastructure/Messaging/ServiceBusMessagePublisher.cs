using System.Text.Json;
using Azure.Messaging.ServiceBus;
using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.DTOs.Boards;
using FlowBoard.Infrastructure.Configurations;
using Microsoft.Extensions.Options;

namespace FlowBoard.Infrastructure.Messaging;

public class ServiceBusMessagePublisher : IArchiveMessagePublisher, IAsyncDisposable
{
    private readonly ServiceBusClient _client;
    private readonly ServiceBusSender _sender;
    private readonly ServiceBusSender _restoreSender;

    public ServiceBusMessagePublisher(IOptions<ServiceBusOptions> options)
    {
        var config = options.Value;
        _client = new ServiceBusClient(config.ConnectionString);
        _sender = _client.CreateSender(config.ArchiveQueueName);
        _restoreSender = _client.CreateSender(config.RestoreQueueName);
    }

    public async Task PublishArchiveMessageAsync(Guid boardId)
    {
        var payload = new BoardArchiveMessage ( BoardId: boardId );

        var json = JsonSerializer.Serialize(payload);

        var message = new ServiceBusMessage(json)
        {
            ContentType = "application/json"
        };

        await _sender.SendMessageAsync(message);
    }

    public async Task PublishRestoreMessageAsync(Guid boardId)
    {
        var payload = new BoardArchiveMessage(BoardId: boardId);
        var json = JsonSerializer.Serialize(payload);
        var message = new ServiceBusMessage(json) 
        { 
            ContentType = "application/json" 
        };

        await _restoreSender.SendMessageAsync(message);
    }

    public async ValueTask DisposeAsync()
    {
        await _sender.DisposeAsync();
        await _restoreSender.DisposeAsync();
        await _client.DisposeAsync();
    }
}