using Orditor.Model;

namespace Orditor.ViewModels;

internal class IntegerInventoryItemViewModel : InventoryItemViewModel<bool>
{
  public IntegerInventoryItemViewModel(Inventory inventory, string propertyName)
    : base(inventory, propertyName)
  {
  }
}