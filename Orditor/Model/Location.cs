using System.Diagnostics;

namespace Orditor.Model;

[DebuggerDisplay("{Name} {X} {Y}")]
internal class Location
{
  public Location(string name, int x, int y)
  {
    Name = name;
    X = x;
    Y = y;
  }

  public string Name { get; }
  public int X { get; protected set; }
  public int Y { get; protected set; }
}