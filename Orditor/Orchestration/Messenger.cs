using System.Collections.Generic;
using Orditor.Model;

namespace Orditor.Orchestration;

internal class Messenger
{
  public void AreasChanged()
  {
    foreach (var listener in _areasChangeListeners)
    {
      listener.AreasChanged();
    }
  }

  public void InventoryChanged(Inventory inventory, string origin)
  {
    foreach (var listener in _inventoryListeners)
    {
      listener.Changed(inventory, origin);
    }
  }

  public void Listen(IInventoryListener listener)
  {
    _inventoryListeners.Add(listener);
  }

  public void Listen(IRestrictedGraphListener listener)
  {
    _restrictedGraphListeners.Add(listener);
  }

  public void Listen(ISelectionListener listener)
  {
    _selectionListeners.Add(listener);
  }

  public void Listen(IAreasListener listener)
  {
    _areasChangeListeners.Add(listener);
  }

  public void RestrictedGraphChanged()
  {
    foreach (var listener in _restrictedGraphListeners)
    {
      listener.Changed();
    }
  }

  public void Select(Pickup pickup)
  {
    foreach (var listener in _selectionListeners)
    {
      listener.Selected(pickup);
    }
  }

  public void Select(Home home)
  {
    foreach (var listener in _selectionListeners)
    {
      listener.Selected(home);
    }
  }

  public void Select(Home home1, Home home2)
  {
    foreach (var listener in _selectionListeners)
    {
      listener.Selected(home1, home2);
    }
  }

  public void Select(Home home, Pickup pickup)
  {
    foreach (var listener in _selectionListeners)
    {
      listener.Selected(home, pickup);
    }
  }

  public void StopListening(ISelectionListener listener)
  {
    _selectionListeners.Remove(listener);
  }

  public void StopListening(IRestrictedGraphListener listener)
  {
    _restrictedGraphListeners.Remove(listener);
  }

  private readonly List<IAreasListener> _areasChangeListeners = new();
  private readonly List<IInventoryListener> _inventoryListeners = new();
  private readonly List<IRestrictedGraphListener> _restrictedGraphListeners = new();
  private readonly List<ISelectionListener> _selectionListeners = new();
}