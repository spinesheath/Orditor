using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Orditor.Model;
using Orditor.Orchestration;

namespace Orditor.ViewModels;

internal class InventoryViewModel : NotificationObject
{
  public InventoryViewModel(Inventory inventory, Messenger messenger, OriginSelectorViewModel originSelector)
  {
    _messenger = messenger;
    _inventory = inventory;

    OriginSelector = originSelector;
    Skills = Listen(Observable(SkillNames.Select(Boolean)));
    Teleporters = Listen(Observable(TpNames.Select(Boolean)));
    WorldEvents = Listen(Observable(WorldEventNames.Select(Boolean)));
    Modifiers = Listen(Observable(ModifierNames.Select(Boolean)));
    Resources = Listen(Observable(ResourceNames.Select(Integer)));
    LogicSets = Listen(Observable(LogicSetNames.Select(Boolean)));

    originSelector.PropertyChanged += OnChange;
  }

  public ObservableCollection<BooleanInventoryItemViewModel> LogicSets { get; }

  public ObservableCollection<BooleanInventoryItemViewModel> Modifiers { get; }

  public OriginSelectorViewModel OriginSelector { get; }

  public ObservableCollection<IntegerInventoryItemViewModel> Resources { get; }

  public ObservableCollection<BooleanInventoryItemViewModel> Skills { get; }

  public ObservableCollection<BooleanInventoryItemViewModel> Teleporters { get; }

  public ObservableCollection<BooleanInventoryItemViewModel> WorldEvents { get; }

  private static readonly List<string> TpNames = new()
  {
    nameof(Inventory.TpGlades),
    nameof(Inventory.TpGrove),
    nameof(Inventory.TpSwamp),
    nameof(Inventory.TpGrotto),
    nameof(Inventory.TpGinso),
    nameof(Inventory.TpValley),
    nameof(Inventory.TpForlorn),
    nameof(Inventory.TpSorrow),
    nameof(Inventory.TpHoruFields),
    nameof(Inventory.TpHoru),
    nameof(Inventory.TpBlackroot),
    nameof(Inventory.TpLostGrove)
  };

  private static readonly List<string> ResourceNames = new()
  {
    nameof(Inventory.Health),
    nameof(Inventory.Energy),
    nameof(Inventory.AbilityCells),
    nameof(Inventory.Keystones),
    nameof(Inventory.MapFragments)
  };

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

  private static readonly List<string> WorldEventNames = new()
  {
    nameof(Inventory.GinsoKey),
    nameof(Inventory.CleanWater),
    nameof(Inventory.ForlornKey),
    nameof(Inventory.WindRestored),
    nameof(Inventory.HoruKey)
  };

  private static readonly List<string> ModifierNames = new()
  {
    nameof(Inventory.OpenWorld),
    nameof(Inventory.OpenDungeons)
  };

  private static readonly List<string> LogicSetNames = new()
  {
    nameof(Inventory.Casual),
    nameof(Inventory.Standard),
    nameof(Inventory.Expert),
    nameof(Inventory.Master),
    nameof(Inventory.Insane),
    nameof(Inventory.Glitched),
    nameof(Inventory.Dboost),
    nameof(Inventory.Lure),
    nameof(Inventory.Gjump),
    nameof(Inventory.Dbash),
    nameof(Inventory.TimedLevel),
  };

  private readonly Inventory _inventory;
  private readonly Messenger _messenger;

  private ObservableCollection<BooleanInventoryItemViewModel> Listen(ObservableCollection<BooleanInventoryItemViewModel> list)
  {
    foreach (var vm in list)
    {
      vm.PropertyChanged += OnChange;
    }

    return list;
  }

  private ObservableCollection<IntegerInventoryItemViewModel> Listen(ObservableCollection<IntegerInventoryItemViewModel> list)
  {
    foreach (var vm in list)
    {
      vm.PropertyChanged += OnChange;
    }

    return list;
  }

  private void OnChange(object? sender, PropertyChangedEventArgs e)
  {
    if (e.PropertyName is nameof(InventoryItemViewModel<object>.Value) or nameof(OriginSelector.Origin))
    {
      _messenger.InventoryChanged(_inventory, OriginSelector.Origin);
    }
  }

  private BooleanInventoryItemViewModel Boolean(string n)
  {
    return new BooleanInventoryItemViewModel(_inventory, n);
  }

  private IntegerInventoryItemViewModel Integer(string n)
  {
    return new IntegerInventoryItemViewModel(_inventory, n);
  }
}