using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace Orditor.Model;

internal class Annotations
{
  public Annotations()
  {
    _annotations = ReadAnnotations();
  }

  public (int, int) Location(string home)
  {
    var element = Element(home);
    var x = ReadInt(element, "x");
    var y = ReadInt(element, "y");
    return (x, y);
  }

  private const string ResourceName = "Orditor.Data.annotations.xml";
  private readonly XElement _annotations;

  private static int ReadInt(XElement? element, string name)
  {
    var value = element?.Attribute(name)?.Value;
    return value == null ? int.MaxValue : Convert.ToInt32(value, CultureInfo.InvariantCulture);
  }

  private XElement? Element(string home)
  {
    return _annotations.Elements("home").FirstOrDefault(e => e.Attribute("name")?.Value == home);
  }

  private static XElement ReadAnnotations()
  {
    var assembly = typeof(Annotations).Assembly;
    using var stream = assembly.GetManifestResourceStream(ResourceName);
    return XElement.Load(stream!);
  }
}