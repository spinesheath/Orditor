using Orditor.Model;
using Orditor.Orchestration;

namespace Orditor.ViewModels;

internal class WorldViewModel : NotificationObject
{
  public WorldViewModel(World world, Selection selection)
  {
    Selection = selection;
    World = world;
  }

  public Selection Selection { get; }

  public World World { get; }
}