namespace RustServerStatus.Models;

public class ServerInfo
{
    public string Name { get; set; }

    public string Map { get; set; }

    public string Game { get; set; }

    public string Address { get; set; }

    public byte PlayerCount { get; set; }

    public byte PlayerCapacity { get; set; }

    public byte BotCount { get; set; }
}