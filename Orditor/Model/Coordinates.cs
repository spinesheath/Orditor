using System.Linq;
using System.Windows;

namespace Orditor.Model;

public static class Coordinates
{
  public static Vector GameToMap(Vector gamePoint)
  {
    var game1 = GameTopLeft;
    var game2 = GameBottomRight;

    var map1 = MapTopLeft;
    var map2 = MapBottomRight;

    var gameVec = game1 - game2;
    var mapVec = map1 - map2;

    var scaleX = mapVec.X / gameVec.X;
    var scaleY = mapVec.Y / gameVec.Y;

    var centered = gamePoint - game1;
    return new Vector(centered.X * scaleX, centered.Y * scaleY) + map1;
  }

  public static Vector MapToGame(Vector mapPoint)
  {
    var game1 = GameTopLeft;
    var game2 = GameBottomRight;

    var map1 = MapTopLeft;
    var map2 = MapBottomRight;

    var gameVec = game1 - game2;
    var mapVec = map1 - map2;

    var scaleX = gameVec.X / mapVec.X;
    var scaleY = gameVec.Y / mapVec.Y;

    var centered = mapPoint - map1;
    return new Vector(centered.X * scaleX, centered.Y * scaleY) + game1;
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
}