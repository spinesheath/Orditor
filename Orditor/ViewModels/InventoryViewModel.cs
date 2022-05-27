using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Orditor.Model;
using Orditor.Orchestration;

namespace Orditor.ViewModels;

internal class InventoryViewModel : NotificationObject
{
  public InventoryViewModel(Messenger messenger)
  {
    _messenger = messenger;
    _inventory = Inventory.Default();

    Skills = Observable(SkillNames.Select(Boolean));
    LogicSets = Observable(LogicSetNames.Select(Boolean));

    foreach (var skill in Skills)
    {
      skill.Value = true;
    }

    ShowReachable = new DelegateCommand(ExecuteShowReachable);
  }

  public ObservableCollection<BooleanInventoryItemViewModel> LogicSets { get; }

  public DelegateCommand ShowReachable { get; }

  public ObservableCollection<BooleanInventoryItemViewModel> Skills { get; }

  private static readonly List<string> SkillNames = new()
  {
    nameof(Inventory.WallJump),
    nameof(Inventory.ChargeFlame),
    nameof(Inventory.DoubleJump),
    nameof(Inventory.Bash),
    nameof(Inventory.Stomp),
    nameof(Inventory.Glide),
    nameof(Inventory.Climb),
    nameof(Inventory.ChargeJump),
    nameof(Inventory.Dash),
    nameof(Inventory.Grenade)
  };

  private static readonly List<string> LogicSetNames = new()
  {
    nameof(Inventory.CasualCore),
    nameof(Inventory.CasualDboost),
    nameof(Inventory.StandardCore),
    nameof(Inventory.StandardDboost),
    nameof(Inventory.StandardLure),
    nameof(Inventory.StandardAbilities),
    nameof(Inventory.ExpertCore),
    nameof(Inventory.ExpertDboost),
    nameof(Inventory.ExpertLure),
    nameof(Inventory.ExpertAbilities),
    nameof(Inventory.Dbash),
    nameof(Inventory.MasterCore),
    nameof(Inventory.MasterDboost),
    nameof(Inventory.MasterLure),
    nameof(Inventory.MasterAbilities),
    nameof(Inventory.Gjump),
    nameof(Inventory.Glitched),
    nameof(Inventory.TimedLevel),
    nameof(Inventory.Insane)
  };

  private readonly Inventory _inventory;
  private readonly Messenger _messenger;

  private BooleanInventoryItemViewModel Boolean(string n)
  {
    return new BooleanInventoryItemViewModel(_inventory, n);
  }

  private static ObservableCollection<BooleanInventoryItemViewModel> Observable(IEnumerable<BooleanInventoryItemViewModel> items)
  {
    return new ObservableCollection<BooleanInventoryItemViewModel>(items);
  }

  private void ExecuteShowReachable()
  {
    _messenger.InventoryChanged(_inventory);
  }
}