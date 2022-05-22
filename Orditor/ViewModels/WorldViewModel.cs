using System;
using Orditor.Model;
using Orditor.Orchestration;

namespace Orditor.ViewModels;

internal class WorldViewModel : NotificationObject, IChangeListener
{
  private readonly PickupGraphParser _parser;

  public WorldViewModel(PickupGraph graph, AreasOri areas, Messenger messenger, PickupGraphParser parser)
  {
    _parser = parser;
    Graph = graph;
    Messenger = messenger;
    Areas = areas;
  }

  public AreasOri Areas { get; }
  public PickupGraph Graph { get; private set; }
  public Messenger Messenger { get; }

  public void Changed()
  {
    PickupGraph graph;
    try
    {
      graph = _parser.Parse(Areas.Text);
    }
    catch
    {
      return;
    }

    Graph = graph;
    OnPropertyChanged(nameof(Graph));
  }
}