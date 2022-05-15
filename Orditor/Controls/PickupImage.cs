using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Orditor.Model;
using Orditor.Orchestration;

namespace Orditor.Controls;

internal class PickupImage : Image
{
  public PickupImage(Pickup pickup, Selection selection)
  {
    _pickup = pickup;
    _selection = selection;
    var imageName = PickupImageName(pickup);

    Width = 30;
    Height = 30;
    Source = new BitmapImage(new Uri($"pack://application:,,,/Orditor;component/Data/{imageName}"));
    ToolTip = pickup.Name;
  }

  private readonly Pickup _pickup;
  private readonly Selection _selection;

  protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
  {
    _selection.Set(_pickup);
  }

  private static string PickupImageName(Pickup pickup)
  {
    return pickup.VanillaContent switch
    {
      var a when a.StartsWith("EX") => "xp.png",
      var b when b.StartsWith("SK") => "tree.png",
      "KS" => "ks.png",
      "AC" => "ac.png",
      "EC" => "ec.png",
      "HC" => "hc.png",
      "MS" => "ms.png",
      "MapStone" => "pedestal.png",
      "Plant" => "plant.png",
      "EVGinsoKey" => "watervein.png",
      "EVForlornKey" => "gumonseal.png",
      "EVHoruKey" => "sunstone.png",
      "EVWater" => "cleanwater.png",
      "EVWind" => "windrestored.png",
      "CS" => "warmth.png",
      _ => "windrestored.png"
    };
  }
}