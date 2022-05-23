using System;
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
  private Connection(Messenger messenger, Home location1, Home location2)
  {
    _selectionSetter = () => messenger.Select(location1, location2);
    var coordinates1 = Coordinates.GameToMap(location1.X, location1.Y);
    var coordinates2 = Coordinates.GameToMap(location2.X, location2.Y);

    _marker = GetMarker(coordinates1, coordinates2);
    Children.Add(_marker);
  }

  private Connection(Messenger messenger, Home location1, Pickup location2)
  {
    _selectionSetter = () => messenger.Select(location1, location2);
    var coordinates1 = Coordinates.GameToMap(location1.X, location1.Y);
    var coordinates2 = Coordinates.GameToMap(location2.X, location2.Y);

    _marker = GetMarker(coordinates1, coordinates2);
    Children.Add(_marker);
  }

  public static Connection Bad(Messenger messenger, Home home, Home target)
  {
    var connection = new Connection(messenger, home, target);
    connection._marker.StrokeDashArray = DashArray;
    return connection;
  }

  public static Connection Good(Messenger messenger, Home home, Home target)
  {
    return new Connection(messenger, home, target);
  }

  public static Connection Good(Messenger messenger, Home home, Pickup target)
  {
    return new Connection(messenger, home, target);
  }

  private static readonly DoubleCollection DashArray = new() { 5, 2 };
  private readonly Line _marker;
  private readonly Action _selectionSetter;

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
    _selectionSetter();
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