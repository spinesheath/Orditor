using System.Collections.Generic;
using System.Linq;
using Orditor.Model;
using Orditor.Orchestration;

namespace Orditor.Reachability;

internal class RestrictedGraph : IChangeListener
{
  public RestrictedGraph(AreasOri areas, PickupGraphParser parser)
  {
    _areas = areas;
    _parser = parser;
    _graph = parser.Parse(areas.Text);
    Origin = _graph.Homes.First();
    ReachableHomes = Enumerable.Empty<Home>();
    UnreachableHomes = Enumerable.Empty<Home>();
    ReachablePickups = Enumerable.Empty<Pickup>();
    UnreachablePickups = Enumerable.Empty<Pickup>();
    ReachableConnections = Enumerable.Empty<RestrictedConnection>();
    UnreachableConnections = Enumerable.Empty<RestrictedConnection>();

    Changed();
  }

  public Home Origin { get; private set; }

  public IEnumerable<RestrictedConnection> ReachableConnections { get; private set; }

  public IEnumerable<Home> ReachableHomes { get; private set; }

  public IEnumerable<Pickup> ReachablePickups { get; private set; }

  public IEnumerable<RestrictedConnection> UnreachableConnections { get; private set; }

  public IEnumerable<Home> UnreachableHomes { get; private set; }

  public IEnumerable<Pickup> UnreachablePickups { get; private set; }

  public void Changed()
  {
    _graph = _parser.Parse(_areas.Text);

    Origin = _graph.Homes.First(h => h.Name == "SunkenGladesRunaway");
    ReachableHomes = _graph.Homes.ToList();
    UnreachableHomes = Enumerable.Empty<Home>();
    ReachablePickups = _graph.Pickups.ToList();
    UnreachablePickups = Enumerable.Empty<Pickup>();

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

  private readonly AreasOri _areas;
  private readonly PickupGraphParser _parser;
  private PickupGraph _graph;
}