using System.Collections.Generic;

namespace Orditor.Controls;

internal class CompletionCandidate
{
  public CompletionCandidate(params string[] fragments)
  {
    Value = fragments[0];
    Fragments = fragments;
  }

  public string Value { get; }

  public string[] Fragments { get; }
}

internal static class CompletionCandidates
{
  public static readonly List<string> Keywords = new()
  {
    "Free",
    "WallJump",
    "ChargeFlame",
    "DoubleJump",
    "Bash",
    "Stomp",
    "Glide",
    "Climb",
    "ChargeJump",
    "Grenade",
    "Dash",
    "Water",
    "Wind",
    "GinsoKey",
    "ForlornKey",
    "HoruKey",
    "TPGrove",
    "TPSwamp",
    "TPGrotto",
    "TPValley",
    "TPSorrow",
    "TPGinso",
    "TPForlorn",
    "TPHoru",
    "Mapstone",
    "OpenWorld",
    "Open"
  };

  public static readonly List<CompletionCandidate> Connection = new()
  {
    new("pickup:"),
    new("conn:")
  };

  public static readonly List<CompletionCandidate> Logic = new()
  {
    new("casual-core", "casual", "core", "cc"),
    new("casual-dboost", "casual", "dboost", "damage", "cd"),
    new("standard-core", "standard", "core", "sc"),
    new("standard-dboost", "standard", "dboost", "damage", "sd"),
    new("standard-lure", "standard", "lure", "sl"),
    new("standard-abilities", "standard", "abilities", "ability", "sa"),
    new("expert-core", "expert", "core", "ec"),
    new("expert-dboost", "expert", "dboost", "damage", "ed"),
    new("expert-lure", "expert", "lure", "el"),
    new("expert-abilities", "expert", "abilities", "ability", "ea"),
    new("dbash", "expert", "bash", "eb"),
    new("master-core", "master", "core"),
    new("master-dboost", "master", "dboost", "damage", "md"),
    new("master-lure", "master", "lure", "ml"),
    new("master-abilities", "master", "abilities", "ability", "ma"),
    new("gjump", "master", "grenade"),
    new("glitched"),
    new("timed-level", "tl"),
    new("insane")
  };
}