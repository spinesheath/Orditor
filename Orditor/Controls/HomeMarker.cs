using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Orditor.Model;
using Orditor.Orchestration;

namespace Orditor.Controls;

internal class HomeMarker : Border
{
  public HomeMarker(Home home, Messenger messenger)
  {
    _messenger = messenger;
    _marker = CreateMarker(home.Name);
    Home = home;

    Width = Radius;
    Height = Radius;
  }

  public Home Home { get; }

  private const int Radius = 15;

  private readonly Messenger _messenger;
  private readonly Ellipse _marker;

  protected override void OnInitialized(EventArgs e)
  {
    base.OnInitialized(e);
    Child = _marker;
  }

  protected override void OnMouseEnter(MouseEventArgs e)
  {
    base.OnMouseEnter(e);
    _marker.Stroke = Brushes.CadetBlue;
    _marker.Fill = Brushes.CadetBlue;
  }

  protected override void OnMouseLeave(MouseEventArgs e)
  {
    base.OnMouseLeave(e);
    _marker.Stroke = Brushes.White;
    _marker.Fill = Brushes.White;
  }

  private static Ellipse CreateMarker(string home)
  {
    var marker = new Ellipse();
    marker.Width = Radius;
    marker.Height = Radius;
    marker.Stroke = Brushes.White;
    marker.StrokeThickness = 1;
    marker.Fill = Brushes.White;
    marker.ToolTip = home;
    return marker;
  }

  protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
  {
    _messenger.Select(Home);
  }
}