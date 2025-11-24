using System.Collections.Concurrent;
using System.Text;

namespace SOA.NotificationService.Hubs;

public class GpaSseHub
{
    private readonly ConcurrentDictionary<Guid, StreamWriter> _clients = new();

    public Guid RegisterClient(Stream responseStream)
    {
        var id = Guid.NewGuid();
        var writer = new StreamWriter(responseStream, new UTF8Encoding(false));
        _clients[id] = writer;

        return id;
    }


    public void UnregisterClient(Guid id) => _clients.TryRemove(id, out _);

    public async Task BroadcastAsync(string json, CancellationToken cancellationToken = default)
    {
        foreach (var (id, writer) in _clients.ToArray())
        {
            try
            {
                await writer.WriteAsync($"data: {json}\n\n");
                await writer.FlushAsync(cancellationToken);
            }
            catch
            {
                _clients.TryRemove(id, out _);
            }
        }
    }
}
