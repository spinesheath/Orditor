using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace Orditor.Model;

internal class Annotations
{
  public Annotations(PickupGraph graph)
  {
    _graph = graph;
    _annotations = ReadAnnotations();
    CalculateHomeLocations();
  }

  public Vector Location(Home home)
  {
    var homeElement = Annotation(home);
    var x = homeElement?.Attribute("x")?.Value;
    var y = homeElement?.Attribute("y")?.Value;

    if (x == null || y == null)
    {
      return CalculatedLocation(home);
    }

    return new Vector(Convert.ToDouble(x, CultureInfo.InvariantCulture), Convert.ToDouble(y, CultureInfo.InvariantCulture));
  }

  private const string ResourceName = "Orditor.Data.annotations.xml";
  private readonly XElement _annotations;
  private readonly Dictionary<Home, Vector> _calculatedLocations = new();
  private readonly PickupGraph _graph;

  private Vector CalculatedLocation(Home home)
  {
    return _calculatedLocations.TryGetValue(home, out var location) ? location : new Vector(double.NaN, double.NaN);
  }

  private XElement? Annotation(Home home)
  {
    return _annotations.Elements("home").FirstOrDefault(e => e.Attribute("name")?.Value == home.Name);
  }

  private static XElement ReadAnnotations()
  {
    var assembly = typeof(Annotations).Assembly;
    using var stream = assembly.GetManifestResourceStream(ResourceName);
    return XElement.Load(stream!);
  }

  private void CalculateHomeLocations()
  {
    var noPickups = new Queue<Home>();
    foreach (var home in _graph.Homes)
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

    while (noPickups.Count > 0)
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
}