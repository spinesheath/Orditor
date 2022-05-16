using Orditor.Model;
using Orditor.Orchestration;

namespace Orditor.ViewModels;

internal class ConnectionEditorViewModel : NotificationObject, ISelectionListener
{
  private string _selectedText = string.Empty;
  private string _rawText = string.Empty;
  private Home? _selectedHome = null;
  private readonly World _world;
  private int _focusedLineIndex;

  public ConnectionEditorViewModel(World world)
  {
    _world = world;
    RawText = world.RawText();
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

  public int FocusedLineIndex
  {
    get => _focusedLineIndex;
    set
    {
      if (_focusedLineIndex != value)
      {
        _focusedLineIndex = value;
        OnPropertyChanged();
      }
    }
  }

  private void UpdateWorld()
  {
  }

  public void Selected(Home home)
  {
    _selectedHome = home;
    SelectedText = home.Name;
    DisplayTextFor(home);
  }

  private void DisplayTextFor(Home home)
  {
    FocusedLineIndex = _world.LineIndex(home);
  }

  public void Selected(Pickup pickup)
  {
    SelectedText = pickup.Name;
    _selectedHome = null;
  }

  public void Selected(Home home1, Home home2)
  {
    SelectedText = $"{home1.Name} - {home2.Name}";
    _selectedHome = null;
  }

  public void Selected(Home home1, Pickup pickup)
  {
    SelectedText = $"{home1.Name} > {pickup.Name}";
    _selectedHome = null;
  }
}