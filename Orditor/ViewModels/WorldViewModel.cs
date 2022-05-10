using Orditor.Model;

namespace Orditor.ViewModels;

internal class WorldViewModel : NotificationObject
{
  public WorldViewModel()
  {
    World = new World();
  }

  public World World { get; }
}