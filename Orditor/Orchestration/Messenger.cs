using System.Collections.Generic;
using Orditor.Model;

namespace Orditor.Orchestration;

internal class Messenger
{
  public void AreasChanged()
  {
    foreach (var listener in _areasChangeListeners)
    {
      listener.Changed();
    }
  }

  public void Listen(ISelectionListener listener)
  {
    _selectionListeners.Add(listener);
  }

  public void ListenForAreas(IChangeListener listener)
  {
    _areasChangeListeners.Add(listener);
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

  private readonly List<IChangeListener> _areasChangeListeners = new();
  private readonly List<ISelectionListener> _selectionListeners = new();
}