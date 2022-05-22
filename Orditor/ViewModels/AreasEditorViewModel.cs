using Orditor.Model;
using Orditor.Orchestration;

namespace Orditor.ViewModels;

internal class AreasEditorViewModel : NotificationObject, IChangeListener
{
  public AreasEditorViewModel(AreasOri areas, Messenger messenger)
  {
    Messenger = messenger;
    _areas = areas;
    RawText = areas.RawText;
  }

  public Messenger Messenger { get; }

  public string RawText
  {
    get => _areas.RawText;
    set => _areas.RawText = value;
  }

  public void Changed()
  {
    OnPropertyChanged(nameof(RawText));
  }

  private readonly AreasOri _areas;
}