using System.Collections.Generic;
using System.Linq;

namespace Orditor.Model;

internal class PickupGraph
{
  public IEnumerable<Connection> Connections => _connections;
  public IEnumerable<Home> Homes => _nameToHome.Values;
  public int NodeCount => _nameToPickup.Count + _nameToHome.Count;
  public IEnumerable<Pickup> Pickups => _nameToPickup.Values;

  public void Add(Pickup pickup)
  {
    _nameToPickup.Add(pickup.Name, pickup);
  }

  public void Add(Home home)
  {
    _nameToHome.Add(home.Name, home);
  }

  public void Add(IEnumerable<Pickup> pickups)
  {
    foreach (var pickup in pickups)
    {
      Add(pickup);
    }
  }

  public void ConnectHome(string home, string target, Requirements requirement)
  {
    _connections.Add(new Connection(home, target, requirement));
  }

  public void ConnectPickup(string home, string pickup, Requirements requirement)
  {
    _connections.Add(new Connection(home, pickup, requirement));
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

  public Home? Home(string name)
  {
    return _nameToHome.TryGetValue(name, out var home) ? home : null;
  }

  public Pickup? Pickup(string name)
  {
    return _nameToPickup.TryGetValue(name, out var pickup) ? pickup : null;
  }

  private readonly List<Connection> _connections = new();
  private readonly Dictionary<string, Home> _nameToHome = new();
  private readonly Dictionary<string, Pickup> _nameToPickup = new();
}