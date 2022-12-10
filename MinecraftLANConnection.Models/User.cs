namespace MinecraftLANConnection.Models;

public sealed class User
{

#nullable enable

    public List<Server?>? Servers { get; private set; }
    public string? Ip { get; private set; }
    public string? Name { get; private set; }

    public User(string ip, string name)
    {
        Ip = ip;
        Name = name;
    }
}
