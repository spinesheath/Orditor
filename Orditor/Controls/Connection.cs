using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Orditor.Model;
using Orditor.Orchestration;
using Orditor.Reachability;

namespace Orditor.Controls;

internal class Connection : Canvas
{
  private Connection(Messenger messenger, Location location1, Location location2, bool arrow, bool traversable)
  {
    _messenger = messenger;
    _location1 = location1;
    _location2 = location2;
    _traversable = traversable;

    var coordinates1 = Coordinates.GameToMap(location1.X, location1.Y);
    var coordinates2 = Coordinates.GameToMap(location2.X, location2.Y);
    _line = CreateLine(coordinates1, coordinates2);
    Children.Add(_line);
    if (arrow)
    {
      _arrowHead = CreateArrowHead(coordinates1, coordinates2);
      Children.Add(_arrowHead);
    }

    Paint(false);
  }

  public static UIElement Bidirectional(Messenger messenger, RestrictedConnection connection)
  {
    return new Connection(messenger, connection.Location1, connection.Location2, false, connection.Traversable);
  }

  public static Connection Unidirectional(Messenger messenger, RestrictedConnection connection)
  {
    var c = new Connection(messenger, connection.Location1, connection.Location2, true, connection.Traversable);
    c._line.StrokeDashArray = DashArray;
    return c;
  }

  private static readonly DoubleCollection DashArray = new() { 5, 2 };
  private readonly Arrow? _arrowHead;
  private readonly Line _line;
  private readonly Location _location1;
  private readonly Location _location2;
  private readonly Messenger _messenger;
  private readonly bool _traversable;

  private static Arrow CreateArrowHead(Vector coordinates1, Vector coordinates2)
  {
    var arrow = new Arrow(new Point(coordinates1.X, coordinates1.Y), new Point(coordinates2.X, coordinates2.Y));
    arrow.StrokeThickness = 3;
    return arrow;
  }

  protected override void OnMouseEnter(MouseEventArgs e)
  {
    base.OnMouseEnter(e);
    Paint(true);
  }

  protected override void OnMouseLeave(MouseEventArgs e)
  {
    base.OnMouseLeave(e);
    Paint(false);
  }

  protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
  {
    if (_location1 is Pickup p1 && _location2 is Home h2)
    {
      _messenger.Select(h2, p1);
    }
    else if (_location1 is Home h1 && _location2 is Pickup p2)
    {
      _messenger.Select(h1, p2);
    }
    else if (_location1 is Home h3 && _location2 is Home h4)
    {
      _messenger.Select(h3, h4);
    }
  }

  private void Paint(bool highlighted)
  {
    if (highlighted)
    {
      if (_traversable)
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
      if (_traversable)
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
    _line.Stroke = brush;
    if (_arrowHead != null)
    {
      _arrowHead.Stroke = brush;
      _arrowHead.Fill = brush;
    }
  }

  private Line CreateLine(Vector location1, Vector location2)
  {
    var connection = new Line
    {
      X1 = location1.X,
      Y1 = location1.Y,
      X2 = location2.X,
      Y2 = location2.Y,
      StrokeThickness = 3,
      Tag = this
    };

    return connection;
  }

  private class Arrow : Shape
  {
    static Arrow()
    {
      Rotate1 = new Matrix();
      Rotate1.Rotate(20.0);
      Rotate2 = new Matrix();
      Rotate2.Rotate(-20.0);
    }

    public Arrow(Point origin, Point target)
    {
      _geometry = new PathGeometry();
      _geometry.Figures.Add(ArrowHead(origin, target));
      _geometry.Freeze();
    }

    protected override Geometry DefiningGeometry => _geometry;

    private static readonly Matrix Rotate1;
    private static readonly Matrix Rotate2;
    private readonly PathGeometry _geometry;

    private static PathFigure ArrowHead(Point origin, Point target)
    {
      var direction = origin - target;
      direction.Normalize();

      var arrowLength = direction * 7;
      var startPoint = target + direction * 10;

      var head = new PathFigure();
      head.IsClosed = true;
      head.StartPoint = startPoint + arrowLength * Rotate1;

      var segment = new PolyLineSegment();
      segment.Points.Add(startPoint);
      segment.Points.Add(startPoint + arrowLength * Rotate2);
      segment.Freeze();
      head.Segments.Add(segment);
      head.Freeze();
      return head;
    }
  }
}