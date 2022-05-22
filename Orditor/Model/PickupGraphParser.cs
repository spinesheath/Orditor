using System;
using System.Collections.Generic;

namespace Orditor.Model;

internal class PickupGraphParser
{
  public PickupGraphParser(Annotations annotations)
  {
    _annotations = annotations;
  }

  public PickupGraph Parse(string text)
  {
    var lines = text.Split(Environment.NewLine);
    var graph = new PickupGraph();
    graph.Add(ReadPickups(lines));
    ReadGraph(lines, graph);
    return graph;
  }

  private readonly Annotations _annotations;

  private void ReadGraph(IEnumerable<string> lines, PickupGraph graph)
  {
    var linesForCurrentHome = new Queue<string>();
    Home? home = null;
    foreach (var line in lines)
    {
      var possibleHome = LineParser.TryHome(line, _annotations);
      if (possibleHome != null)
      {
        if (home != null)
        {
          Commit(home, linesForCurrentHome, graph);
        }

        home = possibleHome;
        linesForCurrentHome.Clear();
      }
      else
      {
        linesForCurrentHome.Enqueue(line);
      }
    }

    if (home != null)
    {
      Commit(home, linesForCurrentHome, graph);
    }
  }

  private static void Commit(Home home, Queue<string> lineQueue, PickupGraph graph)
  {
    graph.Add(home);

    string? pickup = null;
    string? connection = null;

    while (lineQueue.Count > 0)
    {
      var line = lineQueue.Dequeue();

      var newPickup = LineParser.TryPickupReference(line);
      var newConnection = LineParser.TryConnection(line);
      var newRequirement = LineParser.TryRequirement(line);

      var nextIsNotRequirement = lineQueue.Count == 0 || LineParser.TryRequirement(lineQueue.Peek()) == null;

      if (newPickup != null)
      {
        if (nextIsNotRequirement)
        {
          graph.ConnectPickup(home.Name, newPickup, Requirements.Free);
        }

        pickup = newPickup;
        connection = null;
      }
      else if (newConnection != null)
      {
        if (nextIsNotRequirement)
        {
          graph.ConnectHome(home.Name, newConnection, Requirements.Free);
        }

        connection = newConnection;
        pickup = null;
      }
      else if (newRequirement != null)
      {
        if (pickup != null)
        {
          graph.ConnectPickup(home.Name, pickup, newRequirement);
        }
        else if (connection != null)
        {
          graph.ConnectHome(home.Name, connection, newRequirement);
        }
      }
    }
  }

  private static IEnumerable<Pickup> ReadPickups(IEnumerable<string> lines)
  {
    foreach (var line in lines)
    {
      var pickup = LineParser.TryPickupDefinition(line);
      if (pickup != null)
      {
        yield return pickup;
      }
    }
  }
}