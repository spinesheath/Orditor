using Orditor.Model;

namespace Orditor.Orchestration;

internal interface ISelectionListener
{
  void Selected(Home home) {}
  void Selected(Pickup pickup) {}
  void Selected(Home home1, Home home2) {}
  void Selected(Home home1, Pickup pickup) {}
}