using System;
using System.Windows.Input;

namespace Orditor.Controls.Commands;

internal abstract class BaseCommand : ICommand
{
  public bool CanExecute(object? parameter)
  {
    return true;
  }

  public void Execute(object? parameter)
  {
    Execute();
  }

  public event EventHandler? CanExecuteChanged
  {
    add { }
    remove { }
  }

  protected abstract void Execute();
}