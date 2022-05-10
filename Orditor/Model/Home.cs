using System.Collections.Generic;
using System.Linq;

namespace Orditor.Model;

internal class Home
{
  public string Name { get; }
  public string Anchor => Comments.FirstOrDefault(c => c.Contains("anchor"))?.Trim() ?? "unknownanchor";
  public IEnumerable<string> Comments => _comments;
  public IEnumerable<Connection> Connections => _connections;

  public Home(string name)
  {
    Name = name;
  }

  public void Add(Connection c)
  {
    _connections.Add(c);
  }

  public void AddComments(IEnumerable<string> comment)
  {
    _comments.AddRange(comment);
  }

  public bool IsConnectedTo(Home home)
  {
    return Connections.Any(c => !c.IsPickup && c.Target == home.Name);
  }

  public bool IsConnectedTo(Pickup pickup)
  {
    return Connections.Any(c => c.IsPickup && c.Target == pickup.Name);
  }

  private readonly List<Connection> _connections = new();
  private readonly List<string> _comments = new();

  public override string ToString()
  {
    return Name;
  }
}