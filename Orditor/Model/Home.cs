using System.Collections.Generic;

namespace Orditor.Model;

internal class Home
{
  public Home(string name, int x, int y)
  {
    Name = name;
    X = x;
    Y = y;
  }

  public IEnumerable<Connection> Connections => _connections;
  public string Name { get; }
  public int X { get; }
  public int Y { get; }

  public void Add(Connection c)
  {
    _connections.Add(c);
  }

  public override string ToString()
  {
    return Name;
  }

  private readonly List<Connection> _connections = new();
}