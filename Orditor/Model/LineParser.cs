using System;
using System.Text.RegularExpressions;

namespace Orditor.Model;

internal static class LineParser
{
  public static string? TryConnection(string line)
  {
    var match = ConnectionRegex.Match(line);
    return match.Success ? match.Groups[1].Value : null;
  }

  public static string? TryHome(string line)
  {
    var match = HomeRegex.Match(line);
    return match.Success ? match.Groups[1].Value : null;
  }

  public static Pickup? TryPickupDefinition(string line)
  {
    var match = PickupDefinitionRegex.Match(line);
    if (!match.Success)
    {
      return null;
    }

    var name = match.Groups[1].Value;
    var x = Convert.ToInt32(match.Groups[2].Value);
    var y = Convert.ToInt32(match.Groups[3].Value);
    var vanillaContent = match.Groups[4].Value;
    var difficulty = match.Groups[5].Value;
    var zone = match.Groups[6].Value;

    return new Pickup(name, zone, x, y, vanillaContent, difficulty);
  }

  public static string? TryPickupReference(string line)
  {
    var match = PickupReferenceRegex.Match(line);
    return match.Success ? match.Groups[1].Value : null;
  }

  public static Requirements? TryRequirement(string line)
  {
    var match = RequirementRegex.Match(line);
    return match.Success ? new Requirements(match.Groups[0].Value) : null;
  }

  //loc: FirstPickup 92 -227 EX15 0 Glades
  private static readonly Regex PickupDefinitionRegex =
    new(@"^\s*loc:\s+(\w+)\s+([-\d]+)\s+([-\d]+)\s+(\w+)\s+(\d+)\s+(\w+)");

  //home: SunkenGladesRunaway
  private static readonly Regex HomeRegex = new(@"^\s*home:\s+(\w+)");

  //pickup: FirstPickup
  private static readonly Regex PickupReferenceRegex = new(@"^\s*pickup:\s+(\w+)");

  //conn: GladesMain
  private static readonly Regex ConnectionRegex = new(@"^\s*conn:\s+(\w+)");

  //expert-dboost Grenade Health=6
  private static readonly Regex RequirementRegex = new(@"^\s*([-\w]+)\s+(.*)$");
}