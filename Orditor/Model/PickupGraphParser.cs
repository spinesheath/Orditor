using System;
using System.Collections.Generic;

namespace Orditor.Model;

internal class PickupGraphParser
{
  public PickupGraphParser(string text)
  {
    var lines = text.Split(Environment.NewLine);
    ReadPickups(lines);
    ReadGraph(lines);
  }

  public PickupGraph Graph { get; } = new();
  
  private void ReadGraph(IEnumerable<string> lines)
  {
    var linesForCurrentHome = new Queue<string>();
    Home? home = null;
    foreach (var line in lines)
    {
      var possibleHome = LineParser.TryHome(line);
      if (possibleHome != null)
      {
        if (home != null)
        {
          Commit(home, linesForCurrentHome);
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
      Commit(home, linesForCurrentHome);
    }
  }

  private void Commit(Home home, Queue<string> lineQueue)
  {
    Graph.Add(home);

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
          Graph.ConnectPickup(home.Name, newPickup, Requirements.Free);
        }

        pickup = newPickup;
        connection = null;
      }
      else if (newConnection != null)
      {
        if (nextIsNotRequirement)
        {
          Graph.ConnectHome(home.Name, newConnection, Requirements.Free);
        }

        connection = newConnection;
        pickup = null;
      }
      else if (newRequirement != null)
      {
        if (pickup != null)
        {
          Graph.ConnectPickup(home.Name, pickup, newRequirement);
        }
        else if (connection != null)
        {
          Graph.ConnectHome(home.Name, connection, newRequirement);
        }
      }
    }
  }

  private void ReadPickups(IEnumerable<string> lines)
  {
    foreach (var line in lines)
    {
      var pickup = LineParser.TryPickupDefinition(line);
      if (pickup != null)
      {
        Graph.Add(pickup);
      }
    }
  }
}