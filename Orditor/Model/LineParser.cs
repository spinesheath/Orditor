using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Orditor.Model;

internal static class LineParser
{
  public static bool IsConnection(string line)
  {
    return ConnectionRegex.Match(line).Success;
  }

  public static bool IsHome(string line)
  {
    return HomeRegex.Match(line).Success;
  }

  public static bool IsPickupReference(string line)
  {
    return PickupReferenceRegex.Match(line).Success;
  }

  public static string? TryConnection(string line)
  {
    var match = ConnectionRegex.Match(line);
    return match.Success ? match.Groups[1].Value : null;
  }

  public static Home? TryHome(string line)
  {
    var match = HomeRegex.Match(line);
    if (!match.Success)
    {
      return null;
    }

    var name = match.Groups[1].Value;
    var x = ToInt(match.Groups[2]);
    var y = ToInt(match.Groups[3]);

    return new Home(name, x, y);
  }

  private static int ToInt(Capture capture)
  {
    return string.IsNullOrEmpty(capture.Value) ? 0 : Convert.ToInt32(capture.Value, CultureInfo.InvariantCulture);
  }

  public static Pickup? TryPickupDefinition(string line)
  {
    var match = PickupDefinitionRegex.Match(line);
    if (!match.Success)
    {
      return null;
    }

    var name = match.Groups[1].Value;
    var x = ToInt(match.Groups[2]);
    var y = ToInt(match.Groups[3]);
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

  //home: SunkenGladesRunaway *111 111
  private static readonly Regex HomeRegex = new(@"^\s*home:\s+(\w+)(?:\s+([-\d]+)\s+([-\d]+))?");

  //pickup: FirstPickup
  private static readonly Regex PickupReferenceRegex = new(@"^\s*pickup:\s+(\w+)");

  //conn: GladesMain
  private static readonly Regex ConnectionRegex = new(@"^\s*conn:\s+(\w+)");

  //expert-dboost Grenade Health=6
  private static readonly Regex RequirementRegex = new(@"^\s*([-\w]+)\s+(.*)$");
}