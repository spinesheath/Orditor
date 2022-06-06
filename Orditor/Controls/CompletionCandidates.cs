namespace Orditor.Controls;

internal static class CompletionCandidates
{
  public static readonly CompletionCandidate[] Requirements =
  {
    new("Free"),
    new("WallJump", "jump", "wj"),
    new("ChargeFlame", "flame", "cf"),
    new("DoubleJump", "jump", "dj"),
    new("Bash"),
    new("Stomp"),
    new("Glide"),
    new("Climb"),
    new("ChargeJump", "jump", "cj"),
    new("Grenade"),
    new("Dash"),
    new("Water", "clean"),
    new("Wind", "restored"),
    new("GinsoKey", "water", "vein", "wv"),
    new("ForlornKey", "gumon", "seal", "gs"),
    new("HoruKey", "sunstone", "ss"),
    new("TPGrove", "grove"),
    new("TPSwamp", "swamp"),
    new("TPGrotto", "grotto"),
    new("TPValley", "valley"),
    new("TPSorrow", "sorrow"),
    new("TPGinso", "ginso"),
    new("TPForlorn", "forlorn"),
    new("TPHoru", "horu"),
    new("Mapstone", "map", "ms"),
    new("OpenWorld", "world"),
    new("Open", "dungeons"),
    new("Health=", "hc"),
    new("Energy=", "ec"),
    new("Ability=", "ac"),
    new("Keystone=", "ks"),
    new("Mapstone", "ms"),
    new("Ability=3", "airdash", "ad", "burn"),
    new("Ability=6", "chargedash", "cd", "rocketjump", "meteorkick"),
    new("Ability=12", "triplejump", "tj", "ultradefense", "defense", "ud")
  };

  public static readonly CompletionCandidate[] Connection =
  {
    new("pickup:"),
    new("conn:")
  };

  public static readonly CompletionCandidate[] Logic =
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
    new("master-core", "master", "core", "mc"),
    new("master-dboost", "master", "dboost", "damage", "md"),
    new("master-lure", "master", "lure", "ml"),
    new("master-abilities", "master", "abilities", "ability", "ma"),
    new("gjump", "master", "grenade"),
    new("glitched"),
    new("timed-level", "tl"),
    new("insane")
  };
}