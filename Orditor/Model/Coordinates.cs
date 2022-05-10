using System.Windows;

namespace Orditor.Model;

public static class Coordinates
{
  public static Vector GameToMap(Vector gamePoint)
  {
    var game1 = GameStompAreaRoofExp;
    var game2 = GameCjumpTree;

    var map1 = MapStompAreaRoofExp;
    var map2 = MapCjumpTree;

    var gameVec = game1 - game2;
    var mapVec = map1 - map2;

    var scaleX = mapVec.X / gameVec.X;
    var scaleY = mapVec.Y / gameVec.Y;

    var centered = gamePoint - game1;
    return new Vector(centered.X * scaleX, centered.Y * scaleY) + map1;
  }

  public static Vector MapToGame(Vector mapPoint)
  {
    var game1 = GameStompAreaRoofExp;
    var game2 = GameCjumpTree;

    var map1 = MapStompAreaRoofExp;
    var map2 = MapCjumpTree;

    var gameVec = game1 - game2;
    var mapVec = map1 - map2;

    var scaleX = gameVec.X / mapVec.X;
    var scaleY = gameVec.Y / mapVec.Y;

    var centered = mapPoint - map1;
    return new Vector(centered.X * scaleX, centered.Y * scaleY) + game1;
  }

  private static readonly Vector GameStompAreaRoofExp = new(914, -71);
  private static readonly Vector GameCjumpTree = new(-696, 408);
  private static readonly Vector MapStompAreaRoofExp = new(4741, 2380);
  private static readonly Vector MapCjumpTree = new(1206, 1325);
}