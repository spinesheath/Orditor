using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Orditor.Model;
using Orditor.Orchestration;
using Orditor.Reachability;

namespace Orditor.Controls;

[TemplatePart(Name = "PART_GraphCanvas", Type = typeof(Canvas))]
internal class WorldDisplay : Control, IRestrictedGraphListener
{
  static WorldDisplay()
  {
    DefaultStyleKeyProperty.OverrideMetadata(typeof(WorldDisplay), new FrameworkPropertyMetadata(typeof(WorldDisplay)));
  }

  public AreasOri? Areas
  {
    get => (AreasOri?)GetValue(AreasProperty);
    set => SetValue(AreasProperty, value);
  }

  public RestrictedGraph? Graph
  {
    get => (RestrictedGraph?)GetValue(GraphProperty);
    set => SetValue(GraphProperty, value);
  }

  public bool Greyscale
  {
    get => (bool)GetValue(GreyscaleProperty);
    set => SetValue(GreyscaleProperty, value);
  }

  public Messenger? Messenger
  {
    get => (Messenger?)GetValue(MessengerProperty);
    set => SetValue(MessengerProperty, value);
  }

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();

    _graphCanvas = GetTemplateChild("PART_GraphCanvas") as Canvas;

    OnGraphChanged();
  }

  public void Changed()
  {
    OnGraphChanged();
  }

  public static readonly DependencyProperty GreyscaleProperty = DependencyProperty.Register(
    nameof(Greyscale), typeof(bool), typeof(WorldDisplay), new PropertyMetadata(default(bool)));

  public static readonly DependencyProperty GraphProperty = DependencyProperty.Register(
    nameof(Graph), typeof(RestrictedGraph), typeof(WorldDisplay), new PropertyMetadata(default(RestrictedGraph), OnGraphChanged));

  public static readonly DependencyProperty MessengerProperty = DependencyProperty.Register(
    nameof(Messenger), typeof(Messenger), typeof(WorldDisplay), new PropertyMetadata(default(Messenger), OnMessengerChanged));

  public static readonly DependencyProperty AreasProperty = DependencyProperty.Register(
    nameof(Areas), typeof(AreasOri), typeof(WorldDisplay), new PropertyMetadata(default(AreasOri)));

  private Canvas? _graphCanvas;
  private Point _panGripPosition;
  private HomeMarker? _pannedMarker;
  private Vector _panPreviousOffset;

  private static void OnMessengerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    var display = (WorldDisplay)d;

    if (e.OldValue is Messenger oldMessenger)
    {
      oldMessenger.StopListening(display);
    }

    if (e.NewValue is Messenger newMessenger)
    {
      newMessenger.Listen(display);
    }

    display.OnGraphChanged();
  }

  private static void OnGraphChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    var display = (WorldDisplay)d;
    display.OnGraphChanged();
  }

  private void OnGraphChanged()
  {
    if (_graphCanvas == null || Graph == null)
    {
      return;
    }

    ClearMarkers();
    AddConnectionMarkers();
    AddHomeMarkers();
    AddPickupMarkers();
  }

  private void AddConnectionMarkers()
  {
    if (_graphCanvas == null || Graph == null || Messenger == null)
    {
      return;
    }

    foreach (var connection in Graph.ReachableConnections)
    {
      if (connection.Bidirectional)
      {
        _graphCanvas.Children.Add(Connection.Good(Messenger, connection));
      }
      else
      {
        _graphCanvas.Children.Add(Connection.Bad(Messenger, connection));
      }
    }
  }

  private void AddHomeMarkers()
  {
    if (_graphCanvas == null || Graph == null || Messenger == null)
    {
      return;
    }

    foreach (var home in Graph.ReachableHomes)
    {
      var marker = CreateHomeMarker(home, Messenger, true);
      _graphCanvas.Children.Add(marker);
    }

    foreach (var home in Graph.UnreachableHomes)
    {
      var marker = CreateHomeMarker(home, Messenger, false);
      _graphCanvas.Children.Add(marker);
    }
  }

  private HomeMarker CreateHomeMarker(Home home, Messenger messenger, bool reachable)
  {
    var marker = new HomeMarker(home, messenger, reachable);

    marker.MouseLeftButtonDown += OnHomeMouseDown;

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

    return marker;
  }

  private void AddPickupMarkers()
  {
    if (_graphCanvas == null || Graph == null || Messenger == null)
    {
      return;
    }

    foreach (var pickup in Graph.ReachablePickups.Concat(Graph.UnreachablePickups))
    {
      var marker = PickupMarker(pickup, Messenger);
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
      child.MouseLeftButtonDown -= OnHomeMouseDown;
    }

    _graphCanvas.Children.Clear();
  }

  private static UIElement PickupMarker(Pickup pickup, Messenger messenger)
  {
    var marker = new PickupImage(pickup, messenger);
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
    if (_pannedMarker != null && Areas != null)
    {
      var mapPosition = new Vector(Canvas.GetLeft(_pannedMarker) + _pannedMarker.Width / 2, Canvas.GetTop(_pannedMarker) + _pannedMarker.Height / 2);
      var gamePosition = Coordinates.MapToGame(mapPosition);
      var currentPosition = new Vector(_pannedMarker.Home.X, _pannedMarker.Home.Y);
      var d = currentPosition - gamePosition;
      if (d.LengthSquared > 15d)
      {
        var x = (int)Math.Round(gamePosition.X);
        var y = (int)Math.Round(gamePosition.Y);
        Areas.SetLocation(_pannedMarker.Home, x, y);
        Messenger?.Select(_pannedMarker.Home);
      }
    }

    Cursor = Cursors.Arrow;
    _pannedMarker = null;
    ReleaseMouseCapture();
  }
}