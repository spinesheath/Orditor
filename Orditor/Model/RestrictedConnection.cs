namespace Orditor.Model;

internal class RestrictedConnection
{
  public RestrictedConnection(Location location1, Location location2, bool bidirectional)
  {
    Location1 = location1;
    Location2 = location2;
    Bidirectional = bidirectional;
  }

  public bool Bidirectional { get; }
  public Location Location1 { get; }
  public Location Location2 { get; }
}