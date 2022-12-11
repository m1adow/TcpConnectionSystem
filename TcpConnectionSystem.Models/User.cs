using System.Net;
using System.Net.Sockets;

namespace TcpConnectionSystem.Models;

public sealed class User
{

#nullable enable

    public TcpClient? Client { get; private set; } = new TcpClient();
    public TcpListener? Listener { get; private set; }
    public IPAddress? IPAddress { get; private set; }
    public string? Name { get; private set; }

    public User()
    { 
    }

    public User(string name, IPAddress address)
    {
        Name = name;
        IPAddress = address;
        Listener = new TcpListener(address, 8888);
    }
}
