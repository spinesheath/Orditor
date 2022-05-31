using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Orditor.Model;
using Orditor.Orchestration;

namespace Orditor.Controls;

internal class PickupImage : Image
{
  public PickupImage(Pickup pickup, Messenger messenger)
  {
    Pickup = pickup;
    _messenger = messenger;
    var imageName = PickupImageName(pickup);

    Width = 30;
    Height = 30;
    Source = new BitmapImage(new Uri($"pack://application:,,,/Orditor;component/Data/{imageName}"));
    ToolTip = pickup.Name;
  }

  public Pickup Pickup { get; }

  private readonly Messenger _messenger;

  protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
  {
    _messenger.Select(Pickup);
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