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
    _inventory = new Inventory();
    _inventory.Keystones = 100;
    _inventory.AbilityCells = 100;
    _inventory.MapFragments = 100;
    _inventory.Health = 15;
    _inventory.Energy = 15;
    _inventory.CasualCore = true;
    _inventory.CasualDboost = true;
    _inventory.StandardCore = true;
    _inventory.StandardDboost = true;
    _inventory.StandardAbilities = true;
    _inventory.StandardLure = true;

    Skills = Observable(SkillNames.Select(Boolean));

    foreach (var skill in Skills)
    {
      skill.Value = true;
    }

    ShowReachable = new DelegateCommand(ExecuteShowReachable);
  }

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