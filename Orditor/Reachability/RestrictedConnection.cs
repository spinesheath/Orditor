using Orditor.Model;

namespace Orditor.Reachability;

internal class RestrictedConnection
{
  public RestrictedConnection(Location location1, Location location2, bool bidirectional, bool traversable)
  {
    Location1 = location1;
    Location2 = location2;
    Bidirectional = bidirectional;
    Traversable = traversable;
  }

  public bool Bidirectional { get; }
  public Location Location1 { get; }
  public Location Location2 { get; }
  public bool Traversable { get; }
}