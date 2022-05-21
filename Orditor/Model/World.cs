using System.Collections.Generic;
using Orditor.Orchestration;

namespace Orditor.Model;

internal class World
{
  public World(FileManager file, PickupGraph graph)
  {
    _file = file;
    _rawText = _file.Areas;
    _graph = graph;
  }

  public IEnumerable<Home> Homes => _graph.Homes;
  public IEnumerable<Pickup> Pickups => _graph.Pickups;

  public string RawText
  {
    get => _rawText;
    set
    {
      if (_rawText != value)
      {
        _rawText = value;
        _file.Areas = _rawText;
      }
    }
  }

  public IEnumerable<Home> ConnectedHomes(Home home)
  {
    return _graph.GetConnectedHomes(home);
  }

  public IEnumerable<Pickup> ConnectedPickups(Home home)
  {
    return _graph.GetPickups(home);
  }

  public void SetLocation(Home home, int x, int y)
  {
    RawText = LineParser.SetLocation(_rawText, home.Name, x, y);
    home.SetLocation(x, y);
  }
  
  private readonly FileManager _file;
  private readonly PickupGraph _graph;
  private string _rawText;
}