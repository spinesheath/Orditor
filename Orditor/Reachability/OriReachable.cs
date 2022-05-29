using System.Collections.Generic;
using Orditor.Model;

namespace Orditor.Reachability;

internal class OriReachable
{
  public OriReachable(PickupGraph graph, Inventory inventory, Home origin)
  {
    var reachable = new List<Location>(graph.LocationCount) { origin };
    var traversable = new List<Connection>();

    var openWorld = inventory.OpenWorld;
    if (openWorld)
    {
      var gladesMain = graph.Home("GladesMain");
      if (gladesMain != null)
      {
        reachable.Add(gladesMain);
      }
    }

    var reachableById = new bool[graph.LocationCount];

    var spendableKeystoneCount = 0;
    var keystoneDestinations = new List<Location>();
    var keystoneDestinationsById = new bool[graph.LocationCount];

    var newNodes = new Queue<Home>();
    newNodes.Enqueue(origin);
    while (newNodes.Count != 0)
    {
      var home = newNodes.Dequeue();
      var outgoing = graph.Outgoing(home);
      foreach (var connection in outgoing)
      {
        if (inventory.Fulfills(connection.Requirement))
        {
          traversable.Add(connection);
        }

        if (connection.Requirement.Keystone != 0)
        {
          if (!openWorld || connection.Home.Name != "SunkenGladesRunaway" || connection.Target.Name != "GladesMain")
          {
            if (!keystoneDestinationsById[connection.Target.Id])
            {
              keystoneDestinationsById[connection.Target.Id] = true;
              keystoneDestinations.Add(connection.Target);
              spendableKeystoneCount += connection.Requirement.Keystone;
            }
          }
        }
        else if (!reachableById[connection.Target.Id] && inventory.Fulfills(connection.Requirement))
        {
          reachableById[connection.Target.Id] = true;
          reachable.Add(connection.Target);
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
            reachable.Add(destination);

            if (destination is Home target)
            {
              newNodes.Enqueue(target);
            }
          }
        }
      }
    }

    Reachable = reachable;
    Traversable = traversable;
  }

  public IEnumerable<Location> Reachable { get; }

  public IEnumerable<Connection> Traversable { get; }
}