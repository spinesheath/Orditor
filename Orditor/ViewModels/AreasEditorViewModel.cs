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
    get => _rawText;
    set
    {
      if (_rawText != value)
      {
        _rawText = value;
        OnPropertyChanged();
      }
    }
  }

  public void Changed()
  {
    RawText = _areas.RawText;
  }

  private readonly AreasOri _areas;
  private string _rawText = string.Empty;
}