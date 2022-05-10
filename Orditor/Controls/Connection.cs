﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Orditor.Model;

namespace Orditor.Controls;

internal class Connection : Canvas
{
  private readonly Line _marker;

  public Connection(World world, Home location1, Home location2)
  {
    var coordinates1 = Coordinates.GameToMap(world.GetLocation(location1));
    var coordinates2 = Coordinates.GameToMap(world.GetLocation(location2));

    _marker = GetMarker(coordinates1, coordinates2);
    Children.Add(_marker);
  }

  public Connection(World world, Home location1, Pickup location2)
  {
    var coordinates1 = Coordinates.GameToMap(world.GetLocation(location1));
    var coordinates2 = Coordinates.GameToMap(new Vector(location2.X, location2.Y));

    _marker = GetMarker(coordinates1, coordinates2);
    Children.Add(_marker);
  }

  protected override void OnMouseEnter(MouseEventArgs e)
  {
    base.OnMouseEnter(e);
    _marker.StrokeThickness = 6;
  }

  protected override void OnMouseLeave(MouseEventArgs e)
  {
    base.OnMouseLeave(e);
    _marker.StrokeThickness = 3;
  }

  //protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
  //{
  //  _selection.Set(_location1, _location2);
  //}

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