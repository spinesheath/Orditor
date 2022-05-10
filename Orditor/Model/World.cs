using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace Orditor.Model;

internal class World
{
  public IEnumerable<Home> Homes => _graph.Homes;
  public IEnumerable<Pickup> Pickups => _graph.Pickups;

  public Vector GetLocation(Home home)
  {
    var homeElement = GetHomeAnnotation(home);
    var x = homeElement?.Attribute("x")?.Value;
    var y = homeElement?.Attribute("y")?.Value;

    if (x == null || y == null)
    {
      return CalculatedLocation(home);
    }

    return new Vector(Convert.ToDouble(x, CultureInfo.InvariantCulture), Convert.ToDouble(y, CultureInfo.InvariantCulture));
  }

  private Vector CalculatedLocation(Home home)
  {
    return _calculatedLocations.TryGetValue(home, out var location) ? location : new Vector(double.NaN, double.NaN);
  }

  public IEnumerable<Home> GetConnectedHomes(Home home)
  {
    return _graph.GetConnectedHomes(home);
  }

  public IEnumerable<Pickup> GetConnectedPickups(Home home)
  {
    return _graph.GetPickups(home);
  }

  private XElement? GetHomeAnnotation(Home home)
  {
    return _annotations.Elements("home").FirstOrDefault(e => e.Attribute("name")?.Value == home.Name);
  }

  public World()
  {
    var graph = ReadFromAreasOri();
    _graph = graph;

    _annotations = ReadAnnotations();

    CalculateHomeLocations();
  }

  private static XElement ReadAnnotations()
  {
    return File.Exists(AnnotationsPath) ? XElement.Load(AnnotationsPath) : new XElement("annotations");
  }

  private void CalculateHomeLocations()
  {
    var noPickups = new Queue<Home>();
    foreach (var home in Homes)
    {
      var pickups = _graph.GetPickups(home).ToList();
      if (pickups.Count == 0)
      {
        noPickups.Enqueue(home);
      }
      else if (pickups.Count == 1)
      {
        var pickup = pickups[0];
        _calculatedLocations.Add(home, new Vector(pickup.X - 20, pickup.Y));
      }
      else
      {
        var x = pickups.Sum(p => p.X) / pickups.Count;
        var y = pickups.Sum(p => p.Y) / pickups.Count;
        _calculatedLocations.Add(home, new Vector(x, y));
      }
    }

    while(noPickups.Count > 0)
    {
      var home = noPickups.Dequeue();
      var connectedHomes = _graph.GetConnectedHomes(home).ToList();
      if (connectedHomes.Count == 0)
      {
        _calculatedLocations.Add(home, new Vector(50, 50));
        continue;
      }

      var withLocations = connectedHomes.Where(h => _calculatedLocations.ContainsKey(h));
      var locations = withLocations.Select(h => _calculatedLocations[h]).ToList();
        
      if (locations.Count == 0)
      {
        noPickups.Enqueue(home);
      }
      else if (locations.Count == 1)
      {
        var pickup = locations[0];
        _calculatedLocations.Add(home, new Vector(pickup.X + 20, pickup.Y));
      }
      else
      {
        var x = locations.Sum(p => p.X) / locations.Count;
        var y = locations.Sum(p => p.Y) / locations.Count;
        _calculatedLocations.Add(home, new Vector(x, y));
      }
    }
  }

  private static PickupGraph ReadFromAreasOri()
  {
    var lines = ReadAreas();
    var parser = new PickupGraphParser();
    return parser.Parse(lines);
  }

  private static string[] ReadAreas()
  {
    var path = GetPath(AreasOri);
    return File.Exists(path) ? File.ReadAllLines(path) : Array.Empty<string>();
  }

  private static string GetPath(string fileName)
  {
    var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    var snitch = Path.Combine(appData, "Orditor");
    if (!Directory.Exists(snitch))
    {
      Directory.CreateDirectory(snitch);
    }
    return Path.Combine(snitch, fileName);
  }

  private const string AreasOri = "areas.ori";
  private readonly PickupGraph _graph;
  private readonly Dictionary<Home, Vector> _calculatedLocations = new();
  private static readonly string AnnotationsPath = GetPath("annotations.xml");
  private readonly XElement _annotations;
}