using System.Collections.Generic;
using System.Linq;
using Orditor.Model;
using Orditor.Orchestration;

namespace Orditor.Reachability;

internal class RestrictedGraph : IAreasListener, IInventoryListener
{
  public RestrictedGraph(AreasOri areas, PickupGraphParser parser, Messenger messenger)
  {
    _areas = areas;
    _parser = parser;
    _messenger = messenger;
    _graph = parser.Parse(areas.Text);
    Origin = _graph.Homes.First(h => h.Name == "SunkenGladesRunaway");
    ReachableHomes = Enumerable.Empty<Home>();
    UnreachableHomes = Enumerable.Empty<Home>();
    ReachablePickups = Enumerable.Empty<Pickup>();
    UnreachablePickups = Enumerable.Empty<Pickup>();
    ReachableConnections = Enumerable.Empty<RestrictedConnection>();
    UnreachableConnections = Enumerable.Empty<RestrictedConnection>();

    Update();
  }

  public Home Origin { get; private set; }

  public IEnumerable<RestrictedConnection> ReachableConnections { get; private set; }

  public IEnumerable<Home> ReachableHomes { get; private set; }

  public IEnumerable<Pickup> ReachablePickups { get; private set; }

  public IEnumerable<RestrictedConnection> UnreachableConnections { get; private set; }

  public IEnumerable<Home> UnreachableHomes { get; private set; }

  public IEnumerable<Pickup> UnreachablePickups { get; private set; }

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
  private Inventory _inventory = Inventory.Default();

  private void Update(string? origin = null)
  {
    _graph = _parser.Parse(_areas.Text);

    if (origin != null)
    {
      Origin = _graph.Homes.First(h => h.Name == origin);
    }
    
    var reachableLocations = new OriReachable(_graph, _inventory, Origin.Name).ReachableHomes.ToHashSet();

    ReachableHomes = _graph.Homes.Where(h => reachableLocations.Contains(h.Name)).ToList();
    UnreachableHomes = _graph.Homes.Except(ReachableHomes);
    ReachablePickups = _graph.Pickups.Where(p => reachableLocations.Contains(p.Name)).ToList();
    UnreachablePickups = _graph.Pickups.Except(ReachablePickups);

    var connections = new List<RestrictedConnection>();
    foreach (var home in _graph.Homes)
    {
      var connectedHomes = _graph.GetConnectedHomes(home);
      foreach (var target in connectedHomes)
      {
        var bidirectional = _graph.GetConnectedHomes(target).Contains(home);
        connections.Add(new RestrictedConnection(home, target, bidirectional));
      }

      foreach (var pickup in _graph.GetPickups(home))
      {
        connections.Add(new RestrictedConnection(home, pickup, false));
      }
    }

    ReachableConnections = connections;
    UnreachableConnections = Enumerable.Empty<RestrictedConnection>();
  }
}