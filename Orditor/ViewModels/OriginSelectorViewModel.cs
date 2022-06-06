using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Orditor.Model;
using Orditor.Orchestration;
using Orditor.Reachability;

namespace Orditor.ViewModels;

internal class OriginSelectorViewModel : NotificationObject, ISelectionListener, IRestrictedGraphListener
{
  public OriginSelectorViewModel(RestrictedGraph graph)
  {
    _graph = graph;
    SelectOrigin = new DelegateCommand(ExecuteSelectOrigin, () => !_selectingOrigin);
    Homes = Observable(ReadHomes());
    UpdateSummary();
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

  public string Summary { get; private set; } = string.Empty;

  public void GraphChanged()
  {
    var names = ReadHomes();
    var selection = Origin;
    Homes.Update(names);
    RestoreSelection(selection);
    UpdateSummary();
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

  private readonly RestrictedGraph _graph;

  private string _origin = "SunkenGladesRunaway";
  private bool _selectingOrigin;

  private void UpdateSummary()
  {
    Summary = $"{_graph.ReachablePickups.Count()} / {_graph.AllPickups.Count()}";
    OnPropertyChanged(nameof(Summary));
  }

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
    return _graph.AllHomes.Select(h => h.Name).OrderBy(x => x);
  }

  private void ExecuteSelectOrigin()
  {
    _selectingOrigin = true;
    SelectOrigin.RaiseCanExecuteChanged();
  }
}