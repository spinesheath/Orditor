using System.Diagnostics;

namespace Orditor.Model;

[DebuggerDisplay("{Name} {X} {Y}")]
internal class Home
{
  public Home(string name, int x, int y)
  {
    Name = name;
    X = x;
    Y = y;
  }

  public void SetLocation(int x, int y)
  {
    X = x;
    Y = y;
  }
  
  public string Name { get; }
  public int X { get; private set; }
  public int Y { get; private set; }
}