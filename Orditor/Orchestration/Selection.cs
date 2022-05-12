using System.Collections.Generic;
using Orditor.Model;

namespace Orditor.Orchestration;

internal class Selection
{
  public void Listen(ISelectionListener listener)
  {
    _listeners.Add(listener);
  }

  public void Set(Pickup pickup)
  {
    foreach (var listener in _listeners)
    {
      listener.Selected(pickup);
    }
  }

  public void Set(Home home)
  {
    foreach (var listener in _listeners)
    {
      listener.Selected(home);
    }
  }

  public void Set(Home home1, Home home2)
  {
    foreach (var listener in _listeners)
    {
      listener.Selected(home1, home2);
    }
  }

  public void Set(Home home, Pickup pickup)
  {
    foreach (var listener in _listeners)
    {
      listener.Selected(home, pickup);
    }
  }

  private readonly List<ISelectionListener> _listeners = new();
}