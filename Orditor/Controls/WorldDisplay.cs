using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Orditor.Model;

namespace Orditor.Controls;

[TemplatePart(Name = "PART_GraphCanvas", Type = typeof(Canvas))]
internal class WorldDisplay : Control
{
  public static readonly DependencyProperty WorldProperty = DependencyProperty.Register(
    "World", typeof(World), typeof(WorldDisplay), new PropertyMetadata(default(World), OnGraphChanged));

  //public static readonly DependencyProperty SelectionProperty = DependencyProperty.Register(
  //  "Selection", typeof(Selection), typeof(WorldDisplay), new PropertyMetadata(default(Selection)));

  //public static readonly DependencyProperty InputReceiverProperty = DependencyProperty.Register(
  //  "InputReceiver", typeof(IMapInputReceiver), typeof(WorldDisplay), new PropertyMetadata(default(IMapInputReceiver)));

  private Canvas? _graphCanvas;
  //private Point _panGripPosition;
  //private HomeMarker _pannedMarker;
  //private Vector _panPreviousOffset;

  static WorldDisplay()
  {
    DefaultStyleKeyProperty.OverrideMetadata(typeof(WorldDisplay), new FrameworkPropertyMetadata(typeof(WorldDisplay)));
  }

  public World? World
  {
    get => (World?)GetValue(WorldProperty);
    set => SetValue(WorldProperty, value);
  }

  //public Selection Selection
  //{
  //  get => (Selection) GetValue(SelectionProperty);
  //  set => SetValue(SelectionProperty, value);
  //}

  //public IMapInputReceiver InputReceiver
  //{
  //  get => (IMapInputReceiver) GetValue(InputReceiverProperty);
  //  set => SetValue(InputReceiverProperty, value);
  //}

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();

    _graphCanvas = GetTemplateChild("PART_GraphCanvas") as Canvas;

    OnGraphChanged();
  }

  private static void OnGraphChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    var c = (WorldDisplay) d;
    c.OnGraphChanged();
  }

  private void OnGraphChanged()
  {
    if (_graphCanvas == null || World == null)
    {
      return;
    }

    ClearMarkers();
    AddPickupMarkers();
    AddConnectionMarkers();
    AddHomeMarkers();
  }

  private void AddConnectionMarkers()
  {
    if (_graphCanvas == null || World == null)
    {
      return;
    }

    foreach (var home1 in World.Homes)
    {
      var homes = World.GetConnectedHomes(home1);
      foreach (var home2 in homes)
      {
        var connection = new Connection(World, home1, home2);
        _graphCanvas.Children.Add(connection);
      }

      var pickups = World.GetConnectedPickups(home1);
      foreach (var pickup in pickups)
      {
        var connection = new Connection(World, home1, pickup);
        _graphCanvas.Children.Add(connection);
      }
    }
  }

  private void AddHomeMarkers()
  {
    if (_graphCanvas == null || World == null)
    {
      return;
    }

    foreach (var home in World.Homes)
    {
      var marker = new HomeMarker(home);

      //marker.MouseDown += OnHomeMouseDown;

      var location = World.GetLocation(home);
      if (double.IsNaN(location.X))
      {
        Canvas.SetTop(marker, 50 + marker.Height / 2);
        Canvas.SetLeft(marker, 50 + marker.Width / 2);
      }
      else
      {
        var p = Coordinates.GameToMap(location);
        Canvas.SetTop(marker, p.Y - marker.Height / 2);
        Canvas.SetLeft(marker, p.X - marker.Width / 2);
      }

      _graphCanvas.Children.Add(marker);
    }
  }

  private void AddPickupMarkers()
  {
    if (_graphCanvas == null || World == null)
    {
      return;
    }

    foreach (var pickup in World.Pickups)
    {
      var marker = PickupMarker(pickup);
      _graphCanvas.Children.Add(marker);
    }
  }

  private void ClearMarkers()
  {
    //foreach (var child in _graphCanvas.Children.OfType<HomeMarker>())
    //{
    //  child.MouseDown -= OnHomeMouseDown;
    //}

    _graphCanvas?.Children.Clear();
  }

  private static UIElement PickupMarker(Pickup pickup)
  {
    var marker = new PickupImage(pickup);
    var p = Coordinates.GameToMap(new Vector(pickup.X, pickup.Y));
    Canvas.SetTop(marker, p.Y - marker.Height / 2);
    Canvas.SetLeft(marker, p.X - marker.Width / 2);
    return marker;
  }

  //private void OnHomeMouseDown(object sender, MouseButtonEventArgs e)
  //{
  //  var marker = (HomeMarker) sender;
  //  StartPan(marker, e.GetPosition(this));
  //  e.Handled = true;
  //}

  //protected override void OnMouseUp(MouseButtonEventArgs e)
  //{
  //  if (IsPanning())
  //  {
  //    e.Handled = true;
  //    StopPan();
  //  }
  //}

  //protected override void OnMouseMove(MouseEventArgs e)
  //{
  //  if (IsPanning())
  //  {
  //    var panCurrent = e.GetPosition(this);
  //    var newOffset = panCurrent - _panGripPosition + _panPreviousOffset;

  //    Canvas.SetLeft(_pannedMarker, newOffset.X);
  //    Canvas.SetTop(_pannedMarker, newOffset.Y);

  //    e.Handled = true;
  //  }
  //}

  private bool IsPanning()
  {
    return IsMouseCaptured;
  }

  //private void StartPan(HomeMarker marker, Point p)
  //{
  //  _pannedMarker = marker;
  //  _panGripPosition = p;
  //  _panPreviousOffset = new Vector(Canvas.GetLeft(_pannedMarker), Canvas.GetTop(_pannedMarker));
  //  Cursor = Cursors.Hand;
  //  CaptureMouse();
  //}

  //private void StopPan()
  //{
  //  var mapPosition = new Vector(Canvas.GetLeft(_pannedMarker) + _pannedMarker.Width / 2, Canvas.GetTop(_pannedMarker) + _pannedMarker.Height / 2);
  //  var gamePosition = Coordinates.MapToGame(mapPosition);
  //  World.SetLocation(_pannedMarker.Home, gamePosition);

  //  Cursor = Cursors.Arrow;
  //  _pannedMarker = null;
  //  ReleaseMouseCapture();

  //  OnGraphChanged();
  //}
}