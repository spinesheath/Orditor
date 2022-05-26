using System.Diagnostics;

namespace Orditor.Model;

[DebuggerDisplay("{Name} {X} {Y}")]
internal class Location
{
  public Location(string name, int id, int x, int y)
  {
    Name = name;
    Id = id;
    X = x;
    Y = y;
  }

  public string Name { get; }
  public int Id { get; }
  public int X { get; protected set; }
  public int Y { get; protected set; }
}