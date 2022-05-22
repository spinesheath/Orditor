using System.Collections.Generic;
using Orditor.Model;

namespace Orditor.Orchestration;

internal class Messenger
{
  public void Listen(ISelectionListener listener)
  {
    _listeners.Add(listener);
  }

  public void Select(Pickup pickup)
  {
    foreach (var listener in _listeners)
    {
      listener.Selected(pickup);
    }
  }

  public void Select(Home home)
  {
    foreach (var listener in _listeners)
    {
      listener.Selected(home);
    }
  }

  public void Select(Home home1, Home home2)
  {
    foreach (var listener in _listeners)
    {
      listener.Selected(home1, home2);
    }
  }

  public void Select(Home home, Pickup pickup)
  {
    foreach (var listener in _listeners)
    {
      listener.Selected(home, pickup);
    }
  }

  public void StopListening(ISelectionListener listener)
  {
    _listeners.Remove(listener);
  }

  private readonly List<ISelectionListener> _listeners = new();
}