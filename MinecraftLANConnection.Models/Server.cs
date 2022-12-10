namespace MinecraftLANConnection.Models;

public sealed class Server 
{
    public List<User> Users { get; private set; }
    public User Admin { get; private set; }
}
