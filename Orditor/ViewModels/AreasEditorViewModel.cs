using Orditor.Model;
using Orditor.Orchestration;

namespace Orditor.ViewModels;

internal class AreasEditorViewModel : NotificationObject, IChangeListener
{
  public AreasEditorViewModel(AreasOri areas, Messenger messenger)
  {
    Messenger = messenger;
    _areas = areas;
    Text = areas.Text;
  }

  public Messenger Messenger { get; }

  public string Text
  {
    get => _areas.Text;
    set => _areas.Text = value;
  }

  public void Changed()
  {
    OnPropertyChanged(nameof(Text));
  }

  private readonly AreasOri _areas;
}