using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Orditor.Model;
using Orditor.Orchestration;

namespace Orditor.Controls;

internal class Connection : Canvas
{
  private readonly Messenger _messenger;
  private readonly Location _location1;
  private readonly Location _location2;

  private Connection(Messenger messenger, Location location1, Location location2)
  {
    _messenger = messenger;
    _location1 = location1;
    _location2 = location2;

    var coordinates1 = Coordinates.GameToMap(location1.X, location1.Y);
    var coordinates2 = Coordinates.GameToMap(location2.X, location2.Y);
    _marker = GetMarker(coordinates1, coordinates2);
    Children.Add(_marker);
  }

  public static Connection Bad(Messenger messenger, RestrictedConnection connection)
  {
    var c = new Connection(messenger, connection.Location1, connection.Location2);
    c._marker.StrokeDashArray = DashArray;
    return c;
  }
  
  public static UIElement Good(Messenger messenger, RestrictedConnection connection)
  {
    return new Connection(messenger, connection.Location1, connection.Location2);
  }

  private static readonly DoubleCollection DashArray = new() { 5, 2 };
  private readonly Line _marker;

  protected override void OnMouseEnter(MouseEventArgs e)
  {
    base.OnMouseEnter(e);
    _marker.Stroke = Brushes.CadetBlue;
  }

  protected override void OnMouseLeave(MouseEventArgs e)
  {
    base.OnMouseLeave(e);
    _marker.Stroke = Brushes.White;
  }

  protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
  {
    if (_location1 is Pickup p1 && _location2 is Home h2)
    {
      _messenger.Select(h2, p1);
    }
    else if (_location1 is Home h1 && _location2 is Pickup p2)
    {
      _messenger.Select(h1, p2);
    }
    else if (_location1 is Home h3 && _location2 is Home h4)
    {
      _messenger.Select(h3, h4);
    }
  }

  private Line GetMarker(Vector location1, Vector location2)
  {
    var connection = new Line
    {
      X1 = location1.X,
      Y1 = location1.Y,
      X2 = location2.X,
      Y2 = location2.Y,
      Stroke = Brushes.White,
      StrokeThickness = 3,
      Tag = this
    };

    return connection;
  }
}