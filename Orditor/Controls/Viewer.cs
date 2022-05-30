using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Orditor.Controls;

[TemplatePart(Name = "PART_Content", Type = typeof(ContentControl))]
public class Viewer : ContentControl
{
  static Viewer()
  {
    DefaultStyleKeyProperty.OverrideMetadata(typeof(Viewer), new FrameworkPropertyMetadata(typeof(Viewer)));
  }

  public double Zoom
  {
    get => (double)GetValue(ZoomProperty);
    set => SetValue(ZoomProperty, value);
  }

  public double ZoomRate
  {
    get => (double)GetValue(ZoomRateProperty);
    set => SetValue(ZoomRateProperty, value);
  }

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();

    _partContent = GetTemplateChild("PART_Content") as ContentControl;

    Update();
  }

  public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register(
    nameof(Zoom), typeof(double), typeof(Viewer), new PropertyMetadata(1.0, OnZoomChanged));

  public static readonly DependencyProperty ZoomRateProperty = DependencyProperty.Register(
    nameof(ZoomRate), typeof(double), typeof(Viewer), new PropertyMetadata(1.1));

  private Vector _offset;
  private Point _panGripPosition = new(0, 0);
  private Vector _panPreviousOffset = new(0, 0);
  private ContentControl? _partContent;

  private void SetContentRenderTransform(Transform transform)
  {
    if (_partContent == null)
    {
      return;
    }

    _partContent.RenderTransform = transform;
  }

  protected override void OnMouseWheel(MouseWheelEventArgs e)
  {
    var p = e.GetPosition(this);
    var factor = e.Delta > 0 ? ZoomRate : 1 / ZoomRate;
    Scale(factor, new Vector(p.X, p.Y));
  }

  protected override void OnMouseDown(MouseButtonEventArgs e)
  {
    StartPan(e.GetPosition(this));
  }

  protected override void OnMouseUp(MouseButtonEventArgs e)
  {
    if (IsPanning())
    {
      StopPan();
    }
  }

  protected override void OnMouseLeave(MouseEventArgs e)
  {
    StopPan();
    base.OnMouseLeave(e);
  }

  protected override void OnMouseMove(MouseEventArgs e)
  {
    if (IsPanning())
    {
      var panCurrent = e.GetPosition(this);
      var newOffset = panCurrent - _panGripPosition + _panPreviousOffset;
      Translate(newOffset);
    }
  }

  private bool IsPanning()
  {
    return IsMouseCaptured;
  }

  private void StartPan(Point p)
  {
    _panGripPosition = p;
    _panPreviousOffset = _offset;
    Cursor = Cursors.Hand;
    CaptureMouse();
  }

  private void StopPan()
  {
    Cursor = Cursors.Arrow;
    ReleaseMouseCapture();
  }

  private void Translate(Vector offset)
  {
    _offset = offset;
    Update();
  }

  private void Scale(double factor, Vector center)
  {
    _offset = (_offset - center) * factor + center;
    Zoom *= factor;
    Update();
  }

  private void Update()
  {
    var transformGroup = new TransformGroup();
    transformGroup.Children.Add(new ScaleTransform(Zoom, Zoom));
    transformGroup.Children.Add(new TranslateTransform(_offset.X, _offset.Y));
    SetContentRenderTransform(transformGroup);
  }

  private static void OnZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    var c = (Viewer)d;
    c.Update();
  }
}