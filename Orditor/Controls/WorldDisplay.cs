using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Orditor.Model;
using Orditor.Orchestration;

namespace Orditor.Controls;

[TemplatePart(Name = "PART_GraphCanvas", Type = typeof(Canvas))]
internal class WorldDisplay : Control
{
  static WorldDisplay()
  {
    DefaultStyleKeyProperty.OverrideMetadata(typeof(WorldDisplay), new FrameworkPropertyMetadata(typeof(WorldDisplay)));
  }

  public Selection? Selection
  {
    get => (Selection?)GetValue(SelectionProperty);
    set => SetValue(SelectionProperty, value);
  }

  public World? World
  {
    get => (World?)GetValue(WorldProperty);
    set => SetValue(WorldProperty, value);
  }

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();

    _graphCanvas = GetTemplateChild("PART_GraphCanvas") as Canvas;

    OnGraphChanged();
  }

  public static readonly DependencyProperty WorldProperty = DependencyProperty.Register(
    nameof(World), typeof(World), typeof(WorldDisplay), new PropertyMetadata(default(World), OnGraphChanged));

  public static readonly DependencyProperty SelectionProperty = DependencyProperty.Register(
    nameof(Selection), typeof(Selection), typeof(WorldDisplay), new PropertyMetadata(default(Selection), OnGraphChanged));

  private Canvas? _graphCanvas;
  private Point _panGripPosition;
  private HomeMarker? _pannedMarker;
  private Vector _panPreviousOffset;

  private static void OnGraphChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    var c = (WorldDisplay)d;
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
    if (_graphCanvas == null || World == null || Selection == null)
    {
      return;
    }

    foreach (var home1 in World.Homes)
    {
      var homes = World.ConnectedHomes(home1);
      foreach (var home2 in homes)
      {
        var connection = new Connection(Selection, home1, home2);
        _graphCanvas.Children.Add(connection);
      }

      var pickups = World.ConnectedPickups(home1);
      foreach (var pickup in pickups)
      {
        var connection = new Connection(Selection, home1, pickup);
        _graphCanvas.Children.Add(connection);
      }
    }
  }

  private void AddHomeMarkers()
  {
    if (_graphCanvas == null || World == null || Selection == null)
    {
      return;
    }

    foreach (var home in World.Homes)
    {
      var marker = new HomeMarker(home, Selection);

      marker.MouseDown += OnHomeMouseDown;

      if (home.X == int.MaxValue)
      {
        Canvas.SetTop(marker, 50 + marker.Height / 2);
        Canvas.SetLeft(marker, 50 + marker.Width / 2);
      }
      else
      {
        var p = Coordinates.GameToMap(home.X, home.Y);
        Canvas.SetTop(marker, p.Y - marker.Height / 2);
        Canvas.SetLeft(marker, p.X - marker.Width / 2);
      }

      _graphCanvas.Children.Add(marker);
    }
  }

  private void AddPickupMarkers()
  {
    if (_graphCanvas == null || World == null || Selection == null)
    {
      return;
    }

    foreach (var pickup in World.Pickups)
    {
      var marker = PickupMarker(pickup, Selection);
      _graphCanvas.Children.Add(marker);
    }
  }

  private void ClearMarkers()
  {
    if (_graphCanvas == null)
    {
      return;
    }

    foreach (var child in _graphCanvas.Children.OfType<HomeMarker>())
    {
      child.MouseDown -= OnHomeMouseDown;
    }

    _graphCanvas.Children.Clear();
  }

  private static UIElement PickupMarker(Pickup pickup, Selection selection)
  {
    var marker = new PickupImage(pickup, selection);
    var p = Coordinates.GameToMap(pickup.X, pickup.Y);
    Canvas.SetTop(marker, p.Y - marker.Height / 2);
    Canvas.SetLeft(marker, p.X - marker.Width / 2);
    return marker;
  }

  private void OnHomeMouseDown(object sender, MouseButtonEventArgs e)
  {
    var marker = (HomeMarker)sender;
    StartPan(marker, e.GetPosition(this));
    e.Handled = true;
  }

  protected override void OnMouseUp(MouseButtonEventArgs e)
  {
    if (IsPanning())
    {
      e.Handled = true;
      StopPan();
    }
  }

  protected override void OnMouseMove(MouseEventArgs e)
  {
    if (IsPanning() && _pannedMarker != null)
    {
      var panCurrent = e.GetPosition(this);
      var newOffset = panCurrent - _panGripPosition + _panPreviousOffset;

      Canvas.SetLeft(_pannedMarker, newOffset.X);
      Canvas.SetTop(_pannedMarker, newOffset.Y);

      e.Handled = true;
    }
  }

  private bool IsPanning()
  {
    return IsMouseCaptured;
  }

  private void StartPan(HomeMarker marker, Point p)
  {
    _pannedMarker = marker;
    _panGripPosition = p;
    _panPreviousOffset = new Vector(Canvas.GetLeft(_pannedMarker), Canvas.GetTop(_pannedMarker));
    Cursor = Cursors.Hand;
    CaptureMouse();
  }

  private void StopPan()
  {
    if (World != null && _pannedMarker != null)
    {
      var mapPosition = new Vector(Canvas.GetLeft(_pannedMarker) + _pannedMarker.Width / 2, Canvas.GetTop(_pannedMarker) + _pannedMarker.Height / 2);
      var gamePosition = Coordinates.MapToGame(mapPosition);
      var currentPosition = new Vector(_pannedMarker.Home.X, _pannedMarker.Home.Y);
      var d = currentPosition - gamePosition;
      if (d.LengthSquared > 15d)
      {
        var x = (int)Math.Round(gamePosition.X);
        var y = (int)Math.Round(gamePosition.Y);
        World.SetLocation(_pannedMarker.Home, x, y);
      }
    }

    Cursor = Cursors.Arrow;
    _pannedMarker = null;
    ReleaseMouseCapture();

    OnGraphChanged();
  }
}