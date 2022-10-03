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
    new("Mapstone", "ms")
  };

  public static readonly CompletionCandidate[] Connection =
  {
    new("pickup:"),
    new("conn:")
  };

  public static readonly CompletionCandidate[] Logic =
  {
    new("casual"),
    new("standard"),
    new("expert"),
    new("master"),
    new("glitched"),
    new("timed-level", "tl"),
    new("insane")
  };
}