using Orditor.Model;
using Orditor.Orchestration;

namespace Orditor.ViewModels;

internal class ConnectionEditorViewModel : NotificationObject, ISelectionListener
{
  private readonly World _world;
  private string _selectedText = "";

  public ConnectionEditorViewModel(World world)
  {
    _world = world;
  }

  public string SelectedText
  {
    get => _selectedText;
    private set
    {
      if (_selectedText == value)
        return;
      _selectedText = value;
      OnPropertyChanged();
    }
  }

  public void Selected(Home home)
  {
    SelectedText = home.Name;
    var raw = _world.Raw(home);
  }

  public void Selected(Pickup pickup)
  {
    SelectedText = pickup.Name;
  }

  public void Selected(Home home1, Home home2)
  {
    SelectedText = $"{home1.Name} - {home2.Name}";
  }

  public void Selected(Home home1, Pickup pickup)
  {
    SelectedText = $"{home1.Name} > {pickup.Name}";
  }
}