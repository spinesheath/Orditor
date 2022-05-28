using Orditor.Model;

namespace Orditor.ViewModels;

internal class IntegerInventoryItemViewModel : InventoryItemViewModel<int>
{
  public IntegerInventoryItemViewModel(Inventory inventory, string propertyName)
    : base(inventory, propertyName)
  {
  }
}