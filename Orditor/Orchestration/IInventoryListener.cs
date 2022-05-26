using Orditor.Model;

namespace Orditor.Orchestration;

internal interface IInventoryListener
{
  void Changed(Inventory inventory);
}