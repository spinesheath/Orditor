using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Orditor.Model;

namespace Orditor.ViewModels;

internal class InventoryViewModel : NotificationObject
{
  public InventoryViewModel()
  {
    var inventory = new Inventory();
    inventory.Keystones = 100;
    inventory.AbilityCells = 100;
    inventory.MapFragments = 100;
    inventory.Health = 15;
    inventory.Energy = 15;
    inventory.CasualCore = true;
    inventory.CasualDboost = true;
    inventory.StandardCore = true;
    inventory.StandardDboost = true;
    inventory.StandardAbilities = true;
    inventory.StandardLure = true;

    Skills = new ObservableCollection<BooleanInventoryItemViewModel>(SkillNames.Select(n => new BooleanInventoryItemViewModel(inventory, n)));
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

  private void ExecuteShowReachable()
  {
  }
}