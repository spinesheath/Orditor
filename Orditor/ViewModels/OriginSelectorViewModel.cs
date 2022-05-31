using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Orditor.Model;
using Orditor.Orchestration;

namespace Orditor.ViewModels;

internal class OriginSelectorViewModel : NotificationObject, ISelectionListener, IAreasListener
{
  public OriginSelectorViewModel(PickupGraphParser parser, AreasOri areas)
  {
    _parser = parser;
    _areas = areas;
    SelectOrigin = new DelegateCommand(ExecuteSelectOrigin, () => !_selectingOrigin);
    Homes = Observable(ReadHomes());
  }

  public ObservableCollection<string> Homes { get; }

  public string Origin
  {
    get => _origin;
    set
    {
      if (value != _origin)
      {
        _origin = value;
        OnPropertyChanged();
      }
    }
  }

  public DelegateCommand SelectOrigin { get; }

  public void AreasChanged()
  {
    var names = ReadHomes();
    var selection = Origin;
    Homes.Update(names);
    RestoreSelection(selection);
  }

  public void Selected(Home home)
  {
    if (!_selectingOrigin)
    {
      return;
    }

    _selectingOrigin = false;
    Origin = home.Name;
    SelectOrigin.RaiseCanExecuteChanged();
  }

  private readonly AreasOri _areas;
  private readonly PickupGraphParser _parser;
  private string _origin = "SunkenGladesRunaway";
  private bool _selectingOrigin;

  private void RestoreSelection(string selection)
  {
    if (Homes.Contains(selection))
    {
      Origin = selection;
    }
    else if (Homes.Contains("SunkenGladesRunaway"))
    {
      Origin = "SunkenGladesRunaway";
    }
    else
    {
      Origin = Homes.First();
    }
  }

  private IEnumerable<string> ReadHomes()
  {
    var graph = _parser.Parse(_areas.Text);
    return graph.Homes.Select(h => h.Name).OrderBy(x => x);
  }

  private void ExecuteSelectOrigin()
  {
    _selectingOrigin = true;
    SelectOrigin.RaiseCanExecuteChanged();
  }
}