using Orditor.Model;
using Orditor.Orchestration;

namespace Orditor.ViewModels;

internal class WorldViewModel : NotificationObject
{
  public WorldViewModel(PickupGraph graph, AreasOri areas, Messenger messenger)
  {
    Graph = graph;
    Messenger = messenger;
    Areas = areas;
  }

  public AreasOri Areas { get; }
  public PickupGraph Graph { get; }
  public Messenger Messenger { get; }
}