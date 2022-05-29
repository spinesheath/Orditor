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
        _marker.Stroke = GraphColors.AccessibleHighlighted;
        _marker.Fill = GraphColors.AccessibleHighlighted;
      }
      else
      {
        _marker.Stroke = GraphColors.InaccessibleHighlighted;
        _marker.Fill = GraphColors.InaccessibleHighlighted;
      }
    }
    else
    {
      if (_reachable)
      {
        _marker.Stroke = GraphColors.Accessible;
        _marker.Fill = GraphColors.Accessible;
      }
      else
      {
        _marker.Stroke = GraphColors.Inaccessible;
        _marker.Fill = GraphColors.Inaccessible;
      }
    }
  }

  protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
  {
    _messenger.Select(Home);
  }
}