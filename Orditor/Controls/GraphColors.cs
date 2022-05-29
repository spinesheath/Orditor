using System.Windows.Media;

namespace Orditor.Controls;

internal static class GraphColors
{
  public static Brush Accessible { get; } = Brushes.DarkTurquoise;
  public static Brush AccessibleHighlighted { get; } = Brushes.PaleTurquoise;
  public static Brush Inaccessible { get; } = Brushes.OrangeRed;
  public static Brush InaccessibleHighlighted { get; } = Brushes.Orange;
  public static Brush Special { get; } = Brushes.Yellow;
  public static Brush SpecialHighlighted { get; } = Brushes.White;
}