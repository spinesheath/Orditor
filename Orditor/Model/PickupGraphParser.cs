using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Orditor.Model;

internal class PickupGraphParser
{
  public PickupGraphParser(string[] lines)
  {
    ReadPickups(lines);

    HomeData? home = null;

    foreach (var line in lines)
    {
      home = TryUpdate(line, home);
      home?.AddLine(line);
    }

    if (home != null)
    {
      Commit(home);
    }
  }

  public PickupGraph Graph { get; } = new();

  //loc: FirstPickup 92 -227 EX15 0 Glades
  private static readonly Regex PickupDefintionRegex =
    new(@"^\s*loc:\s+(\w+)\s+([-\d]+)\s+([-\d]+)\s+(\w+)\s+(\d+)\s+(\w+)");

  //home: SunkenGladesRunaway
  private static readonly Regex HomeRegex = new(@"^\s*home:\s+(\w+)");

  //pickup: FirstPickup
  private static readonly Regex PickupReferenceRegex = new(@"^\s*pickup:\s+(\w+)");

  //conn: GladesMain
  private static readonly Regex ConnectionRegex = new(@"^\s*conn:\s+(\w+)");

  //expert-dboost Grenade Health=6
  private static readonly Regex RequirementRegex = new(@"^\s*([-\w]+)\s+(.*)$");

  private HomeData? TryUpdate(string line, HomeData? current)
  {
    var match = HomeRegex.Match(line);
    if (match.Success)
    {
      if (current != null)
      {
        Commit(current);
      }

      return new HomeData(match.Groups[1].Value);
    }

    return current;
  }

  private static string? GetPickup(string line)
  {
    var match = PickupReferenceRegex.Match(line);
    return match.Success ? match.Groups[1].Value : null;
  }

  private static string? GetConnection(string line)
  {
    var match = ConnectionRegex.Match(line);
    return match.Success ? match.Groups[1].Value : null;
  }

  private static Requirements? GetRequirement(string line)
  {
    var match = RequirementRegex.Match(line);
    return match.Success ? new Requirements(match.Groups[0].Value) : null;
  }

  private void Commit(HomeData home)
  {
    Graph.CreateHome(home.Name);

    string? pickup = null;
    string? connection = null;

    var todo = new Queue<string>(home.Lines);

    while (todo.Count > 0)
    {
      var current = todo.Dequeue();

      var newPickup = GetPickup(current);
      var newConnection = GetConnection(current);
      var newRequirement = GetRequirement(current);

      var nextIsNotRequirement = todo.Count == 0 || GetRequirement(todo.Peek()) == null;

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
      var match = PickupDefintionRegex.Match(line);
      if (!match.Success)
      {
        continue;
      }

      var name = match.Groups[1].Value;
      var x = Convert.ToInt32(match.Groups[2].Value);
      var y = Convert.ToInt32(match.Groups[3].Value);
      var vanillaContent = match.Groups[4].Value;
      var difficulty = match.Groups[5].Value;
      var zone = match.Groups[6].Value;

      Graph.Add(new Pickup(name, zone, x, y, vanillaContent, difficulty));
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
      if (!HomeRegex.IsMatch(line))
      {
        _lines.Add(line);
      }
    }

    private readonly List<string> _lines = new();
  }
}