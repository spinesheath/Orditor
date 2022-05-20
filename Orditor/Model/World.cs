using System.Collections.Generic;
using System.Windows;

namespace Orditor.Model;

internal class World
{
  public World(string rawText, PickupGraph graph, Annotations annotations)
  {
    _rawText = rawText;
    _graph = graph;
    _annotations = annotations;
  }

  public IEnumerable<Home> Homes => _graph.Homes;
  public IEnumerable<Pickup> Pickups => _graph.Pickups;

  public IEnumerable<Home> ConnectedHomes(Home home)
  {
    return _graph.GetConnectedHomes(home);
  }

  public IEnumerable<Pickup> ConnectedPickups(Home home)
  {
    return _graph.GetPickups(home);
  }

  public Vector Location(Home home)
  {
    return _annotations.Location(home);
  }

  public string RawText()
  {
    return _rawText;
  }

  public void SetLocation(Home home, Vector gamePosition)
  {

  }
  
  private readonly Annotations _annotations;
  private readonly PickupGraph _graph;
  private readonly string _rawText;
}