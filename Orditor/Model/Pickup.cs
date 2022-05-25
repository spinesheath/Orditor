using System;

namespace Orditor.Model;

internal class Pickup : Location
{
  public Pickup(string name, string zone, int x, int y, string vanillaContent, string difficulty)
    : base(name, x, y)
  {
    Id = ToMultipleOfFour(x) * 10000 + ToMultipleOfFour(y);
    Zone = zone;
    VanillaContent = vanillaContent;
    Difficulty = difficulty;
  }

  public string Difficulty { get; }
  public int Id { get; }
  public string VanillaContent { get; }
  public string Zone { get; }

  public override string ToString()
  {
    return $"{Name} {Zone} {Id}";
  }

  private static int ToMultipleOfFour(int x)
  {
    return (int)(Math.Floor(x / 4.0) * 4.0);
  }
}