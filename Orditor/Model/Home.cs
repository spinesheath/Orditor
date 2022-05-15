using System.Collections.Generic;
using System.Linq;

namespace Orditor.Model;

internal class Home
{
  public string Name { get; }
  public IEnumerable<Connection> Connections => _connections;

  public Home(string name)
  {
    Name = name;
  }

  public void Add(Connection c)
  {
    _connections.Add(c);
  }

  private readonly List<Connection> _connections = new();

  public override string ToString()
  {
    return Name;
  }
}