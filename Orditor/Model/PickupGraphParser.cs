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
    HomeData? home = null;
    foreach (var line in lines)
    {
      var name = LineParser.TryHome(line);
      if (name != null)
      {
        if (home != null)
        {
          Commit(home);
        }
        
        home = new HomeData(name);
      }
      else
      {
        home?.AddLine(line);
      }
    }

    if (home != null)
    {
      Commit(home);
    }
  }

  private void Commit(HomeData home)
  {
    Graph.CreateHome(home.Name);

    string? pickup = null;
    string? connection = null;

    var lineQueue = new Queue<string>(home.Lines);

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

  private class HomeData
  {
    public HomeData(string name)
    {
      Name = name;
    }

    public IEnumerable<string> Lines => _lines;

    public string Name { get; }

    public void AddLine(string line)
    {
      _lines.Add(line);
    }

    private readonly List<string> _lines = new();
  }
}