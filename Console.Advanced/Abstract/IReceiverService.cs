namespace Console.Advanced.Abstract;

public interface IReceiverService
{
    Task ReceiveAsync(CancellationToken stoppingToken);
}
