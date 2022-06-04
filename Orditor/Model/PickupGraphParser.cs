using System;
using System.Collections.Generic;
using System.Linq;

namespace Orditor.Model;

internal class PickupGraphParser
{
  public PickupGraphParser(Annotations annotations)
  {
    _annotations = annotations;
  }

  public PickupGraph Parse(string text)
  {
    var lines = text.Split(Environment.NewLine).Select(RemoveCommentAndTrim).Where(line => !string.IsNullOrWhiteSpace(line)).ToList();
    return ReadGraph(lines);
  }

  private readonly Annotations _annotations;

  private static string RemoveCommentAndTrim(string line)
  {
    var commentIndex = line.IndexOf("--", StringComparison.Ordinal);
    var withoutComment = commentIndex >= 0 ? line[..commentIndex] : line;
    return withoutComment.Trim();
  }

  private PickupGraph ReadGraph(List<string> lines)
  {
    var accumulator = new Accumulator();
    accumulator.ReadDefinitions(lines, _annotations);

    var connections = new List<Connection>();
    var linesForCurrentHome = new Queue<string>();
    Home? home = null;

    foreach (var line in lines)
    {
      var possibleHome = accumulator.Home(LineParser.TryHomeName(line));
      if (possibleHome != null)
      {
        if (home != null)
        {
          connections.AddRange(ParseRequirements(home, linesForCurrentHome, accumulator));
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
      connections.AddRange(ParseRequirements(home, linesForCurrentHome, accumulator));
    }

    return new PickupGraph(accumulator.Homes, accumulator.Pickups, connections);
  }

  private static IEnumerable<Connection> ParseRequirements(Home home, Queue<string> lineQueue, Accumulator accumulator)
  {
    Location? currentTarget = null;

    while (lineQueue.Count > 0)
    {
      var line = lineQueue.Dequeue();

      var newPickup = accumulator.Pickup(LineParser.TryPickupReference(line));
      var newConnection = accumulator.Home(LineParser.TryConnection(line));
      var newTarget = (Location?)newPickup ?? newConnection;
      var newRequirement = LineParser.TryRequirement(line);

      var nextIsNotRequirement = lineQueue.Count == 0 || LineParser.TryRequirement(lineQueue.Peek()) == null;

      if (newTarget != null)
      {
        if (nextIsNotRequirement)
        {
          yield return new Connection(home, newTarget, Requirements.Free);
        }

        currentTarget = newTarget;
      }
      else if (newRequirement != null && currentTarget != null)
      {
        yield return new Connection(home, currentTarget, newRequirement);
      }
    }
  }

  private class Accumulator
  {
    public List<Home> Homes { get; } = new();

    public List<Pickup> Pickups { get; } = new();

    public Home? Home(string? name)
    {
      return name == null ? null : Homes.FirstOrDefault(p => p.Name == name);
    }

    public Pickup? Pickup(string? name)
    {
      return name == null ? null : Pickups.FirstOrDefault(p => p.Name == name);
    }

    public void ReadDefinitions(List<string> lines, Annotations annotations)
    {
      var idGenerator = new IdGenerator();

      foreach (var line in lines)
      {
        Add(LineParser.TryHome(line, annotations, idGenerator));
      }

      foreach (var line in lines)
      {
        Add(LineParser.TryPickupDefinition(line, idGenerator));
      }
    }

    private void Add(Pickup? pickup)
    {
      if (pickup != null)
      {
        Pickups.Add(pickup);
      }
    }

    private void Add(Home? home)
    {
      if (home != null)
      {
        Homes.Add(home);
      }
    }
  }
}