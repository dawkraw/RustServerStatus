using System.Net;
using System.Net.Sockets;
using System.Text;
using RustServerStatus.Extensions;
using RustServerStatus.Models;

namespace RustServerStatus.Services;

public class ServerQueryService : IServerQueryService
{
    private static readonly byte[] RequestHeader = {0xFF, 0xFF, 0xFF, 0xFF, 0x54};

    public async Task<ServerInfo?> QueryServerAsync(string address)
    {
        var parsedAddress = ParseAddress(address);
        var isAValidAddress = await IsAValidAddressAsync(parsedAddress.ip);
        if (isAValidAddress is not true) return null;

        var queryPacket = RequestHeader.Concat(Encoding.UTF8.GetBytes("Source Engine Query\0")).ToArray();
        using var udpClient = new UdpClient();
        await udpClient.SendAsync(queryPacket, queryPacket.Length, parsedAddress.ip, parsedAddress.port);
        var response = await udpClient.ReceiveAsyncWithTimeout(5000);

        if (response is null || response.Length < 8) return null;

        if (response.Length == 9) //server sent challenge
        {
            var challenge = response.TakeLast(4);
            queryPacket = queryPacket.Concat(challenge).ToArray();
            await udpClient.SendAsync(queryPacket, queryPacket.Length, parsedAddress.ip, parsedAddress.port);
            response = await udpClient.ReceiveAsyncWithTimeout(5000);
        }

        var info = ParseInfo(response);
        info.Address = address;
        return info;
    }

    private ServerAddress ParseAddress(string address)
    {
        var addressParts = address.Split(":");
        return addressParts.Length == 2
            ? new ServerAddress(addressParts[0], ushort.Parse(addressParts[1]))
            : new ServerAddress(address, 28015);
    }

    private async Task<bool> IsAValidAddressAsync(string address)
    {
        if (IPAddress.TryParse(address, out _))
            return true;
        try
        {
            var hostAddresses = await Dns.GetHostAddressesAsync(address);
            return hostAddresses.Length > 0;
        }
        catch (SocketException)
        {
            return false;
        }
    }

    private ServerInfo ParseInfo(byte[] bytes)
    {
        var info = new ServerInfo();
        
        using var stream = new MemoryStream(bytes);
        using var reader = new BinaryReader(stream);
        
        reader.ReadBytes(6); //headers and protocol, not important
        info.Name = reader.ReadStringWithoutSize();
        info.Map = reader.ReadStringWithoutSize();
        reader.ReadStringWithoutSize(); //folder, also not important
        info.Game = reader.ReadStringWithoutSize();
        reader.ReadInt16(); //steam appid, mostly not used and also not important
        info.PlayerCount = reader.ReadByte();
        info.PlayerCapacity = reader.ReadByte();
        info.BotCount = reader.ReadByte();

        return info;
    }
}