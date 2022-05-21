using System.Linq;
using System.Windows;

namespace Orditor.Model;

public static class Coordinates
{
  public static Vector GameToMap(int x, int y)
  {
    return new Vector((x - GameTopLeft.X) * ScaleX, (y - GameTopLeft.Y) * ScaleY) + MapTopLeft;
  }

  public static Vector MapToGame(Vector v)
  {
    return new Vector((v.X - MapTopLeft.X) / ScaleX, (v.Y - MapTopLeft.Y) / ScaleY) + GameTopLeft;
  }

  private static readonly Vector GameStompAreaRoofExp = new(914, -71);
  private static readonly Vector GameGinsoEscapeHangingExp = new(519, 867);
  private static readonly Vector GameLostGroveLongSwim = new(527, -544);
  private static readonly Vector GameMistyAbilityCell = new(-1075, -2);

  private static readonly Vector[] GameVectors =
  {
    GameStompAreaRoofExp,
    GameGinsoEscapeHangingExp,
    GameLostGroveLongSwim,
    GameMistyAbilityCell
  };

  private static readonly Vector GameTopLeft = new(GameVectors.Min(v => v.X), GameVectors.Max(v => v.Y));
  private static readonly Vector GameBottomRight = new(GameVectors.Max(v => v.X), GameVectors.Min(v => v.Y));

  private static readonly Vector MapStompAreaRoofExp = new(4741, 2380);
  private static readonly Vector MapGinsoEscapeHangingExp = new(3872, 327);
  private static readonly Vector MapLostGroveLongSwim = new(3893, 3424);
  private static readonly Vector MapMistyAbilityCell = new(371, 2232);

  private static readonly Vector[] MapVectors =
  {
    MapStompAreaRoofExp,
    MapGinsoEscapeHangingExp,
    MapLostGroveLongSwim,
    MapMistyAbilityCell
  };

  private static readonly Vector MapTopLeft = new(MapVectors.Min(v => v.X), MapVectors.Min(v => v.Y));
  private static readonly Vector MapBottomRight = new(MapVectors.Max(v => v.X), MapVectors.Max(v => v.Y));

  private static readonly double ScaleX = (MapTopLeft.X - MapBottomRight.X) / (GameTopLeft.X - GameBottomRight.X);
  private static readonly double ScaleY = (MapTopLeft.Y - MapBottomRight.Y) / (GameTopLeft.Y - GameBottomRight.Y);
}