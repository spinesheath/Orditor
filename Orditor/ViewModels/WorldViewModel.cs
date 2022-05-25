using Orditor.Model;
using Orditor.Orchestration;

namespace Orditor.ViewModels;

internal class WorldViewModel : NotificationObject
{
  public WorldViewModel(RestrictedGraph graph, Messenger messenger, AreasOri areas)
  {
    Graph = graph;
    Messenger = messenger;
    Areas = areas;
  }

  public RestrictedGraph Graph { get; }
  public Messenger Messenger { get; }
  public AreasOri Areas { get; }
}