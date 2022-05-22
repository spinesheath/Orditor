using Orditor.Model;
using Orditor.Orchestration;

namespace Orditor.ViewModels;

internal class ConnectionEditorViewModel : NotificationObject, ISelectionListener
{
  public ConnectionEditorViewModel(AreasOri areas, Messenger messenger)
  {
    Messenger = messenger;
    _areas = areas;
    RawText = areas.RawText;
  }

  public string RawText
  {
    get => _rawText;
    set
    {
      if (_rawText != value)
      {
        _rawText = value;
        UpdateWorld();
        OnPropertyChanged();
      }
    }
  }

  public string SelectedText
  {
    get => _selectedText;
    private set
    {
      if (_selectedText != value)
      {
        _selectedText = value;
        OnPropertyChanged();
      }
    }
  }

  public Messenger Messenger { get; }

  public void Selected(Home home)
  {
    SelectedText = home.Name;
  }

  public void Selected(Pickup pickup)
  {
    SelectedText = pickup.Name;
  }

  public void Selected(Home home1, Home home2)
  {
    SelectedText = $"{home1.Name} - {home2.Name}";
  }

  public void Selected(Home home, Pickup pickup)
  {
    SelectedText = $"{home.Name} > {pickup.Name}";
  }

  private readonly AreasOri _areas;
  private string _rawText = string.Empty;
  private string _selectedText = string.Empty;

  private void UpdateWorld()
  {
  }
}