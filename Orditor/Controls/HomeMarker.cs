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
  public HomeMarker(Home home, Selection selection)
  {
    _selection = selection;
    _home = home;

    Width = Radius;
    Height = Radius;
  }

  private const int Radius = 15;

  private readonly Home _home;

  private readonly Selection _selection;

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
    marker.ToolTip = _home.Name;
    Child = marker;
  }

  protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
  {
    _selection.Set(_home);
  }
}