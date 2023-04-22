using System.Collections.Generic;
using System.Linq;
using Orditor.Model;
using Orditor.Orchestration;

namespace Orditor.Reachability;

internal class RestrictedGraph : IAreasListener, IInventoryListener
{
  public RestrictedGraph(AreasOri areas, PickupGraphParser parser, Messenger messenger, Inventory inventory, string origin)
  {
    _areas = areas;
    _parser = parser;
    _messenger = messenger;
    _graph = parser.Parse(areas.Text);
    _inventory = inventory;
    Origin = _graph.Homes.First(h => h.Name == origin);
    ReachableHomes = Enumerable.Empty<Home>();
    UnreachableHomes = Enumerable.Empty<Home>();
    ReachablePickups = Enumerable.Empty<Pickup>();
    ReachableConnections = Enumerable.Empty<RestrictedConnection>();
    UnreachableConnections = Enumerable.Empty<RestrictedConnection>();

    Update();
  }

  public IEnumerable<Home> AllHomes => _graph.Homes;
  
  public IEnumerable<Pickup> AllPickups => _graph.Pickups;

  public Home Origin { get; private set; }

  public IEnumerable<RestrictedConnection> ReachableConnections { get; private set; }

  public IEnumerable<Home> ReachableHomes { get; private set; }

  public IEnumerable<Pickup> ReachablePickups { get; private set; }

  public IEnumerable<RestrictedConnection> UnreachableConnections { get; private set; }

  public IEnumerable<Home> UnreachableHomes { get; private set; }

  public void AreasChanged()
  {
    Update();
    _messenger.RestrictedGraphChanged();
  }

  public void Changed(Inventory inventory, string origin)
  {
    _inventory = inventory;
    Update(origin);
    _messenger.RestrictedGraphChanged();
  }

  private readonly AreasOri _areas;
  private readonly PickupGraphParser _parser;
  private readonly Messenger _messenger;
  private PickupGraph _graph;
  private Inventory _inventory;

  private void Update(string? origin = null)
  {
    _graph = _parser.Parse(_areas.Text);
    
    // Have to look origin up again since it could be a new instance with same name
    Origin = _graph.Homes.First(h => h.Name == (origin ?? Origin.Name));

    var oriReachable = new OriReachable(_graph, _inventory, Origin);
    var reachableLocations = oriReachable.Reachable.ToHashSet();
    
    var traversableByHome = new Dictionary<Home, HashSet<Location>>();
    foreach (var home in _graph.Homes)
    {
      traversableByHome[home] = new HashSet<Location>();
    }

    foreach (var connection in oriReachable.Traversable)
    {
      traversableByHome[connection.Home].Add(connection.Target);
    }

    ReachableHomes = _graph.Homes.Where(h => reachableLocations.Contains(h)).ToList();
    UnreachableHomes = _graph.Homes.Except(ReachableHomes);
    ReachablePickups = _graph.Pickups.Where(p => reachableLocations.Contains(p)).ToList();

    var connections = new List<RestrictedConnection>();
    foreach (var home in _graph.Homes)
    {
      var traversableConnections = traversableByHome[home];
      var connectedHomes = _graph.GetConnectedHomes(home);
      foreach (var target in connectedHomes)
      {
        var traversable = traversableConnections.Contains(target);

        var bidirectional = _graph.GetConnectedHomes(target).Contains(home);

        if (!traversable && bidirectional)
        {
          traversable = traversableByHome[target].Contains(home);
        }

        connections.Add(new RestrictedConnection(home, target, bidirectional, traversable));
      }

      foreach (var pickup in _graph.GetPickups(home))
      {
        var traversable = traversableConnections.Contains(pickup);

        connections.Add(new RestrictedConnection(home, pickup, false, traversable));
      }
    }

    ReachableConnections = connections;
    UnreachableConnections = Enumerable.Empty<RestrictedConnection>();
  }
}