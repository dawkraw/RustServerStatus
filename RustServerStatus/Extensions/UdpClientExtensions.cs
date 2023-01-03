using System.Net.Sockets;

namespace RustServerStatus.Extensions;

public static class UdpClientExtensions
{
    public static async Task<byte[]?> ReceiveAsyncWithTimeout(this UdpClient client, int timeout)
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var response = client.ReceiveAsync(cancellationTokenSource.Token);
        var taskWait = Task.WaitAny(new[] {response.AsTask()}, timeout, cancellationTokenSource.Token);

        if (response.IsCompleted) return response.Result.Buffer;

        cancellationTokenSource.Cancel();
        return null;
    }
}