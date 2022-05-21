using System.Collections.Generic;
using System.Linq;

namespace Orditor.Model;

internal class PickupGraph
{
  public IEnumerable<Home> Homes => _nameToHome.Values;
  public IEnumerable<Pickup> Pickups => _nameToPickup.Values;

  public void Add(Pickup pickup)
  {
    _nameToPickup.Add(pickup.Name, pickup);
  }

  public void Add(Home home)
  {
    if (!_nameToHome.ContainsKey(home.Name))
    {
      _nameToHome.Add(home.Name, home);
    }
  }

  public void ConnectHome(string home, string target, Requirements requirement)
  {
    AddConnection(new Connection(home, target, requirement, false));
  }

  public void ConnectPickup(string home, string pickup, Requirements requirement)
  {
    AddConnection(new Connection(home, pickup, requirement, true));
  }

  public IEnumerable<Home> GetConnectedHomes(Home home)
  {
    return home.Connections.Where(c => !c.IsPickup).Select(p => _nameToHome[p.Target]).Distinct().Where(h => h != home);
  }

  public IEnumerable<Pickup> GetPickups(Home home)
  {
    return home.Connections.Where(c => c.IsPickup).Select(p => _nameToPickup[p.Target]).Distinct();
  }

  private readonly Dictionary<string, Home> _nameToHome = new();
  private readonly Dictionary<string, Pickup> _nameToPickup = new();

  private void AddConnection(Connection connection)
  {
    if (!_nameToHome.ContainsKey(connection.Home))
    {
      _nameToHome.Add(connection.Home, new Home(connection.Home, 0, 0));
    }

    _nameToHome[connection.Home].Add(connection);

    if (!connection.IsPickup && !_nameToHome.ContainsKey(connection.Target))
    {
      _nameToHome.Add(connection.Target, new Home(connection.Target, 0, 0));
    }
  }
}