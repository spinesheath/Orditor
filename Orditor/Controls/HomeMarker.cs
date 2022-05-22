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
    Home = home;

    Width = Radius;
    Height = Radius;
  }

  public Home Home { get; }

  private const int Radius = 15;

  private readonly Messenger _messenger;

  protected override void OnInitialized(EventArgs e)
  {
    base.OnInitialized(e);
    SetMarker();
  }

  private void SetMarker()
  {
    var marker = new Ellipse();
    marker.Width = Radius;
    marker.Height = Radius;
    marker.Stroke = Brushes.White;
    marker.StrokeThickness = 1;
    marker.Fill = Brushes.White;
    marker.ToolTip = Home.Name;
    Child = marker;
  }

  protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
  {
    _messenger.Select(Home);
  }
}