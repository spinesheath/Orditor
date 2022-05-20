using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace Orditor.Model;

internal class World
{
  public World(string rawText, PickupGraph graph)
  {
    _graph = graph;
    _rawText = rawText;
    _annotations = new Annotations(AnnotationsPath, _graph);
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
    _annotations.SetLocation(home, gamePosition);
  }
  
  private static readonly string AnnotationsPath = GetPath("annotations.xml");
  private readonly Annotations _annotations;
  private readonly PickupGraph _graph;
  private readonly string _rawText;

  private static string GetPath(string fileName)
  {
    var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    var directory = Path.Combine(appData, "Orditor");
    if (!Directory.Exists(directory))
    {
      Directory.CreateDirectory(directory);
    }

    return Path.Combine(directory, fileName);
  }
}