using System.Collections.Generic;
using Orditor.Model;

namespace Orditor.Reachability;

internal class OriReachable
{
  private readonly PickupGraph _graph;
  private readonly bool _openWorld;

  public OriReachable(PickupGraph graph, bool openWorld)
  {
    _graph = graph;
    _openWorld = openWorld;
  }

  public List<string> Reachable(Inventory inventory, string originName)
  {
    var reachable = new List<string>(_graph.LocationCount);
    reachable.Add(originName);
    if (_openWorld)
    {
      reachable.Add("GladesMain");
    }

    var reachableById = new bool[_graph.LocationCount];

    var spendableKeystoneCount = 0;
    var keystoneDestinations = new List<Location>();
    var keystoneDestinationsById = new bool[_graph.LocationCount];

    var newNodes = new Queue<Home>();
    var origin = _graph.Home(originName) ?? throw new InvalidGraphException();
    newNodes.Enqueue(origin);
    while (newNodes.Count != 0)
    {
      var home = newNodes.Dequeue();
      var outgoing = _graph.Outgoing(home);
      foreach (var connection in outgoing)
      {
        if (connection.Requirement.Keystone != 0)
        {
          if (!_openWorld || connection.Home.Name != "SunkenGladesRunaway" || connection.Target.Name != "GladesMain")
          {
            if (!keystoneDestinationsById[connection.Target.Id])
            {
              keystoneDestinationsById[connection.Target.Id] = true;
              keystoneDestinations.Add(connection.Target);
              spendableKeystoneCount += connection.Requirement.Keystone;
            }
          }
        }
        else if (!reachableById[connection.Target.Id] &&
                 inventory.Fulfills(connection.Requirement))
        {
          var name = connection.Target.Name;
          reachableById[connection.Target.Id] = true;
          reachable.Add(name);
          if (connection.Target is Home target)
          {
            newNodes.Enqueue(target);
          }
        }
      }

      if (newNodes.Count == 0 && spendableKeystoneCount <= inventory.Keystones)
      {
        foreach (var destination in keystoneDestinations)
        {
          if (!reachableById[destination.Id])
          {
            reachableById[destination.Id] = true;
            reachable.Add(destination.Name);

            if (destination is Home target)
            {
              newNodes.Enqueue(target);
            }
          }
        }
      }
    }

    return reachable;
  }
}