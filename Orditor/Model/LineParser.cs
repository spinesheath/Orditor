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

  public static bool IsHome(string line, string home)
  {
    var match = HomeRegex.Match(line);
    return match.Success && match.Groups[1].Value == home;
  }

  public static bool IsPickupReference(string line)
  {
    return PickupReferenceRegex.Match(line).Success;
  }

  public static string SetLocation(string text, string home, int x, int y)
  {
    var regex = HomeReplacementRegex(home);
    return regex.Replace(text, $"$1 {x} {y}$3");
  }

  public static string? TryConnection(string line)
  {
    var match = ConnectionRegex.Match(line);
    return match.Success ? match.Groups[1].Value : null;
  }

  public static Home? TryHome(string line, Annotations annotations, IdGenerator homeIdGenerator)
  {
    var match = HomeRegex.Match(line);
    if (!match.Success)
    {
      return null;
    }

    var name = match.Groups[1].Value;
    var (ax, ay) = annotations.Location(name);
    var x = ToInt(match.Groups[2], ax);
    var y = ToInt(match.Groups[3], ay);

    return new Home(name, homeIdGenerator.Next(), x, y);
  }

  public static string? TryHomeName(string line)
  {
    var match = HomeRegex.Match(line);
    return match.Success ? match.Groups[1].Value : null;
  }

  public static Pickup? TryPickupDefinition(string line, IdGenerator idGenerator)
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

    return new Pickup(name, idGenerator.Next(), zone, x, y, vanillaContent, difficulty);
  }

  public static string? TryPickupName(string line)
  {
    var match = PickupDefinitionRegex.Match(line);
    return match.Success ? match.Groups[1].Value : null;
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
    new(@"^\s*loc:\s+(\w+)\s+(-?\d+)\s+(-?\d+)\s+(\w+)\s+(\d+)\s+(\w+)");

  //home: SunkenGladesRunaway *111 111
  private static readonly Regex HomeRegex = new(@"^\s*home:\s*(\w+)(?:\s+(-?\d+)\s+(-?\d+))?", RegexOptions.Multiline);

  //pickup: FirstPickup
  private static readonly Regex PickupReferenceRegex = new(@"^\s*pickup:\s*(\w+)", RegexOptions.Multiline);

  //conn: GladesMain
  private static readonly Regex ConnectionRegex = new(@"^\s*conn:\s*(\w+)", RegexOptions.Multiline);

  //expert-dboost Grenade Health=6
  private static readonly Regex RequirementRegex = new(@"^\s*([-\w]+)\s+(.*)$", RegexOptions.Multiline);

  private static Regex HomeReplacementRegex(string home)
  {
    return new Regex($@"^(\s*home:\s*{home})(\s+[-\d]+\s+[-\d]+)?(\s+.*)?$", RegexOptions.Multiline);
  }

  private static int ToInt(Capture capture, int fallback = int.MaxValue)
  {
    return string.IsNullOrEmpty(capture.Value) ? fallback : Convert.ToInt32(capture.Value, CultureInfo.InvariantCulture);
  }
}