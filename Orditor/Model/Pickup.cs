using System;

namespace Orditor.Model;

internal class Pickup
{
  public Pickup(string name, string zone, int x, int y, string vanillaContent, string difficulty)
  {
    Id = ToMultipleOfFour(x) * 10000 + ToMultipleOfFour(y);
    Name = name;
    Zone = zone;
    VanillaContent = vanillaContent;
    Difficulty = difficulty;
    X = x;
    Y = y;
  }

  public string Difficulty { get; }
  public int Id { get; }
  public string Name { get; }
  public string VanillaContent { get; }
  public double X { get; }
  public double Y { get; }
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