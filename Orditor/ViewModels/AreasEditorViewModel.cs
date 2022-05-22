using Orditor.Model;
using Orditor.Orchestration;

namespace Orditor.ViewModels;

internal class AreasEditorViewModel : NotificationObject, IChangeListener
{
  public AreasEditorViewModel(AreasOri areas, Messenger messenger)
  {
    Messenger = messenger;
    _areas = areas;
    RawText = areas.Text;
  }

  public Messenger Messenger { get; }

  public string RawText
  {
    get => _areas.Text;
    set => _areas.Text = value;
  }

  public void Changed()
  {
    OnPropertyChanged(nameof(RawText));
  }

  private readonly AreasOri _areas;
}