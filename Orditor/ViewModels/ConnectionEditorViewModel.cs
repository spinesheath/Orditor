using Orditor.Model;
using Orditor.Orchestration;

namespace Orditor.ViewModels;

internal class ConnectionEditorViewModel : NotificationObject, ISelectionListener
{
  private readonly World _world;
  private string _selectedText = string.Empty;
  private string _raw = string.Empty;

  public ConnectionEditorViewModel(World world)
  {
    _world = world;
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

  public string Raw
  {
    get => _raw;
    set
    {
      if (_raw != value)
      {
        _raw = value;
        OnPropertyChanged();
      }
    }
  }

  public void Selected(Home home)
  {
    SelectedText = home.Name;
    Raw = _world.Raw(home);
  }

  public void Selected(Pickup pickup)
  {
    SelectedText = pickup.Name;
    Raw = string.Empty;
  }

  public void Selected(Home home1, Home home2)
  {
    SelectedText = $"{home1.Name} - {home2.Name}";
    Raw = string.Empty;
  }

  public void Selected(Home home1, Pickup pickup)
  {
    SelectedText = $"{home1.Name} > {pickup.Name}";
    Raw = string.Empty;
  }
}