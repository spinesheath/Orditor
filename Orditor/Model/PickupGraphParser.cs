using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Orditor.Model;

internal class PickupGraphParser
{
  public PickupGraph Parse(string[] lines)
  {
    ReadPickups(lines);

    foreach (var line in lines)
    {
      TryUpdateHome(line);
      _home?.AddLine(line);
    }

    TryCommitHome();

    return _graph;
  }

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

  private readonly PickupGraph _graph = new();
  private HomeData? _home;

  private void TryUpdateHome(string line)
  {
    var match = HomeRegex.Match(line);
    if (match.Success)
    {
      TryCommitHome();
      _home = new HomeData(match.Groups[1].Value);
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

  private void TryCommitHome()
  {
    if (_home == null)
    {
      return;
    }

    _graph.AddComments(_home.Name, _home.Comments);

    string? pickup = null;
    string? connection = null;

    var todo = new Queue<string>(_home.Lines);

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
          _graph.ConnectPickup(_home.Name, newPickup, Requirements.Free);
        }

        pickup = newPickup;
        connection = null;
      }
      else if (newConnection != null)
      {
        if (nextIsNotRequirement)
        {
          _graph.ConnectHome(_home.Name, newConnection, Requirements.Free);
        }

        connection = newConnection;
        pickup = null;
      }
      else if (newRequirement != null)
      {
        if (pickup != null)
        {
          _graph.ConnectPickup(_home.Name, pickup, newRequirement);
        }
        else if (connection != null)
        {
          _graph.ConnectHome(_home.Name, connection, newRequirement);
        }
      }
    }

    _home = null;
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

      _graph.Add(new Pickup(name, zone, x, y, vanillaContent, difficulty));
    }
  }

  private class HomeData
  {
    public HomeData(string name)
    {
      Name = name;
    }

    public IEnumerable<string> Comments => _comments;
    public IEnumerable<string> Lines => _lines;

    public string Name { get; }

    public void AddLine(string line)
    {
      if (IsComment(line))
      {
        _comments.Add(line);
      }
      else if (!HomeRegex.IsMatch(line))
      {
        _lines.Add(line);
      }
    }

    private readonly List<string> _comments = new();
    private readonly List<string> _lines = new();

    private static bool IsComment(string line)
    {
      return line.Trim().StartsWith("--");
    }
  }
}