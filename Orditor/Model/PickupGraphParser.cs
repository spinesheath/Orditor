using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Orditor.Model;

internal class PickupGraphParser
{
  public PickupGraphParser(string[] lines)
  {
    ReadPickups(lines);
    ReadGraph(lines);
  }

  public PickupGraph Graph { get; } = new();

  public StructuredFile File { get; } = new();

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

  private void ReadGraph(string[] lines)
  {
    var preface = new List<string>();
    var homeLines = new List<string>();

    HomeData? home = null;

    foreach (var line in lines)
    {
      var match = HomeRegex.Match(line);
      if (match.Success)
      {
        if (home == null)
        {
          File.AddBlock("preface", string.Join(Environment.NewLine, preface));
        }
        else
        {
          File.AddBlock(home.Name, string.Join(Environment.NewLine, homeLines));
          homeLines.Clear();
          Commit(home);
        }

        home = new HomeData(match.Groups[1].Value);
      }

      if (home != null)
      {
        home.AddLine(line);
        homeLines.Add(line);
      }
      else
      {
        preface.Add(line);
      }
    }

    if (home != null)
    {
      Commit(home);
    }
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

    var lineQueue = new Queue<string>(home.Lines);

    while (lineQueue.Count > 0)
    {
      var line = lineQueue.Dequeue();

      var newPickup = GetPickup(line);
      var newConnection = GetConnection(line);
      var newRequirement = GetRequirement(line);

      var nextIsNotRequirement = lineQueue.Count == 0 || GetRequirement(lineQueue.Peek()) == null;

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