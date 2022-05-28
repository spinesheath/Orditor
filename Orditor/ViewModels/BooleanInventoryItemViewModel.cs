using Orditor.Model;

namespace Orditor.ViewModels;

internal class BooleanInventoryItemViewModel : InventoryItemViewModel<bool>
{
  public BooleanInventoryItemViewModel(Inventory inventory, string propertyName)
    : base(inventory, propertyName)
  {
  }
}