using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
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
  
  private readonly bool _isOrigin;
  private readonly Ellipse _marker;
  private readonly Messenger _messenger;
  private readonly bool _reachable;

  public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register(
    nameof(Radius), typeof(double), typeof(HomeMarker), new PropertyMetadata(15.0d));

  public double Radius
  {
    get => (double)GetValue(RadiusProperty);
    set => SetValue(RadiusProperty, value);
  }

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

  private Ellipse CreateMarker(string home)
  {
    var marker = new Ellipse();
    marker.StrokeThickness = 0;
    marker.ToolTip = home;
    BindRadius(marker, HeightProperty);
    BindRadius(marker, WidthProperty);
    return marker;
  }

  private void BindRadius(Shape target, DependencyProperty property)
  {
    var binding = new Binding(nameof(Radius));
    binding.Source = this;
    target.SetBinding(property, binding);
  }

  private void UpdateColor(bool hovering)
  {
    if (hovering)
    {
      if (_isOrigin)
      {
        _marker.Fill = GraphColors.SpecialHighlighted;
      }
      else if (_reachable)
      {
        _marker.Fill = GraphColors.AccessibleHighlighted;
      }
      else
      {
        _marker.Fill = GraphColors.InaccessibleHighlighted;
      }
    }
    else
    {
      if (_isOrigin)
      {
        _marker.Fill = GraphColors.Special;
      }
      else if (_reachable)
      {
        _marker.Fill = GraphColors.Accessible;
      }
      else
      {
        _marker.Fill = GraphColors.Inaccessible;
      }
    }
  }

  protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
  {
    _messenger.Select(Home);
  }
}