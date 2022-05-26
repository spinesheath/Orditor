using System.Collections.Generic;
using System.Linq;

namespace Orditor.Model;

internal class PickupGraph
{
  public PickupGraph(List<Home> homes, List<Pickup> pickups, List<Connection> connections)
  {
    Homes = homes.ToList();
    Pickups = pickups.ToList();
    LocationCount = homes.Count + pickups.Count;

    var tempConnections = new List<Connection>[homes.Count];
    for (var i = 0; i < homes.Count; i++)
    {
      tempConnections[i] = new List<Connection>();
    }

    foreach (var connection in connections)
    {
      tempConnections[connection.Home.Id].Add(connection);
    }

    _outgoingConnections = new Connection[homes.Count][];
    for (var i = 0; i < homes.Count; i++)
    {
      _outgoingConnections[i] = tempConnections[i].ToArray();
    }
  }

  public Connection[] Outgoing(Home home)
  {
    return _outgoingConnections[home.Id];
  }

  public int LocationCount { get; }
  public IEnumerable<Home> Homes { get; }
  public IEnumerable<Pickup> Pickups { get; }

  public Home? Home(string name)
  {
    return Homes.FirstOrDefault(x => x.Name == name);
  }

  public IEnumerable<Home> GetConnectedHomes(Home home)
  {
    return Outgoing(home).Select(c => c.Target).OfType<Home>().Distinct();
  }

  public IEnumerable<Pickup> GetPickups(Home home)
  {
    return Outgoing(home).Select(c => c.Target).OfType<Pickup>().Distinct();
  }
  
  private readonly Connection[][] _outgoingConnections;
}