using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Orditor.Model;

namespace Orditor.Controls;

internal class HomeMarker : Border
{
  public HomeMarker(Home home)
  {
    //_selection = selection;
    Home = home;

    var marker = new Ellipse();
    marker.Width = Radius;
    marker.Height = Radius;
    marker.Stroke = Brushes.White;
    marker.StrokeThickness = 1;
    marker.Fill = Brushes.White;
    marker.ToolTip = home.Name;

    Width = Radius;
    Height = Radius;

    Child = marker;
  }

  public Home Home { get; }

  //private readonly Selection _selection;
  private const int Radius = 15;

  //protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
  //{
  //  _selection.Set(Home);
  //}
}