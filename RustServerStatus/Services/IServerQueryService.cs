using RustServerStatus.Models;

namespace RustServerStatus.Services;

public interface IServerQueryService
{
    Task<ServerInfo?> QueryServerAsync(string address);
}