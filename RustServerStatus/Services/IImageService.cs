using RustServerStatus.Models;

namespace RustServerStatus.Services;

public interface IImageService
{
    public byte[] GenerateServerInfoImage(ServerInfo serverInfo);
}