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
  public HomeMarker(Home home, Messenger messenger, bool reachable)
  {
    _messenger = messenger;
    _reachable = reachable;
    _marker = CreateMarker(home.Name);
    Home = home;

    Width = Radius;
    Height = Radius;

    UpdateColor(false);
  }

  public Home Home { get; }

  private const int Radius = 15;

  private readonly Messenger _messenger;
  private readonly bool _reachable;
  private readonly Ellipse _marker;

  protected override void OnInitialized(EventArgs e)
  {
    base.OnInitialized(e);
    Child = _marker;
  }

  protected override void OnMouseEnter(MouseEventArgs e)
  {
    base.OnMouseEnter(e);
    UpdateColor(true);
  }

  protected override void OnMouseLeave(MouseEventArgs e)
  {
    base.OnMouseLeave(e);
    UpdateColor(false);
  }

  private static Ellipse CreateMarker(string home)
  {
    var marker = new Ellipse();
    marker.Width = Radius;
    marker.Height = Radius;
    marker.StrokeThickness = 1;
    marker.ToolTip = home;
    return marker;
  }

  private void UpdateColor(bool hovering)
  {
    if (hovering)
    {
      if (_reachable)
      {
        _marker.Stroke = Brushes.CadetBlue;
        _marker.Fill = Brushes.CadetBlue;
      }
      else
      {
        _marker.Stroke = Brushes.OrangeRed;
        _marker.Fill = Brushes.OrangeRed;
      }
    }
    else
    {
      if (_reachable)
      {
        _marker.Stroke = Brushes.White;
        _marker.Fill = Brushes.White;
      }
      else
      {
        _marker.Stroke = Brushes.Orange;
        _marker.Fill = Brushes.Orange;
      }
    }
  }

  protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
  {
    _messenger.Select(Home);
  }
}