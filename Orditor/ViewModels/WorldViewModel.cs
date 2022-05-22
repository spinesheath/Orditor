using Orditor.Model;
using Orditor.Orchestration;

namespace Orditor.ViewModels;

internal class WorldViewModel : NotificationObject
{
  public WorldViewModel(PickupGraph graph, AreasOri areas, Selection selection)
  {
    Graph = graph;
    Selection = selection;
    Areas = areas;
  }

  public AreasOri Areas { get; }
  public PickupGraph Graph { get; }
  public Selection Selection { get; }
}