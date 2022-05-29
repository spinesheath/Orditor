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
  public HomeMarker(Home home, Messenger messenger, bool reachable, bool isOrigin)
  {
    _messenger = messenger;
    _reachable = reachable;
    _isOrigin = isOrigin;
    _marker = CreateMarker(home.Name);
    Home = home;

    Width = Radius;
    Height = Radius;

    UpdateColor(false);
  }

  public Home Home { get; }

  private const int Radius = 15;
  private readonly bool _isOrigin;
  private readonly Ellipse _marker;
  private readonly Messenger _messenger;
  private readonly bool _reachable;

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
      if (_isOrigin)
      {
        Paint(GraphColors.SpecialHighlighted);
      }
      else if (_reachable)
      {
        Paint(GraphColors.AccessibleHighlighted);
      }
      else
      {
        Paint(GraphColors.InaccessibleHighlighted);
      }
    }
    else
    {
      if (_isOrigin)
      {
        Paint(GraphColors.Special);
      }
      else if (_reachable)
      {
        Paint(GraphColors.Accessible);
      }
      else
      {
        Paint(GraphColors.Inaccessible);
      }
    }
  }

  private void Paint(Brush brush)
  {
    _marker.Stroke = brush;
    _marker.Fill = brush;
  }

  protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
  {
    _messenger.Select(Home);
  }
}