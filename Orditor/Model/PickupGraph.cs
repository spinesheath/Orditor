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
    _nameToHome.Add(home.Name, home);
  }

  public void ConnectHome(string home, string target, Requirements requirement)
  {
    _connections.Add(new Connection(home, target, requirement, false));
  }

  public void ConnectPickup(string home, string pickup, Requirements requirement)
  {
    _connections.Add(new Connection(home, pickup, requirement, true));
  }

  public IEnumerable<Home> GetConnectedHomes(Home home)
  {
    return _connections.Where(c => c.Home == home.Name)
      .Where(c => _nameToHome.ContainsKey(c.Target))
      .Select(c => _nameToHome[c.Target])
      .Distinct();
  }

  public IEnumerable<Pickup> GetPickups(Home home)
  {
    return _connections.Where(c => c.Home == home.Name)
      .Where(c => _nameToPickup.ContainsKey(c.Target))
      .Select(c => _nameToPickup[c.Target])
      .Distinct();
  }

  private readonly List<Connection> _connections = new();

  private readonly Dictionary<string, Home> _nameToHome = new();
  private readonly Dictionary<string, Pickup> _nameToPickup = new();
}