using System;
using System.Windows.Input;

namespace Orditor.ViewModels;

internal class DelegateCommand : ICommand
{
  public DelegateCommand(Action execute, Func<bool> canExecute)
  {
    _execute = execute;
    _canExecute = canExecute;
  }

  public DelegateCommand(Action execute)
  {
    _execute = execute;
  }

  public void RaiseCanExecuteChanged()
  {
    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
  }

  public bool CanExecute(object? parameter)
  {
    return _canExecute?.Invoke() ?? true;
  }

  public void Execute(object? parameter)
  {
    _execute();
  }

  public event EventHandler? CanExecuteChanged;
  private readonly Func<bool>? _canExecute;
  private readonly Action _execute;
}

internal class DelegateCommand<T> : ICommand
{
  public DelegateCommand(Action<T> execute, Func<T, bool> canExecute)
  {
    _execute = execute;
    _canExecute = canExecute;
  }

  public DelegateCommand(Action<T> execute)
  {
    _execute = execute;
  }

  public void RaiseCanExecuteChanged()
  {
    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
  }

  public bool CanExecute(object? parameter)
  {
    if (parameter is not T p)
    {
      return false;
    }

    return _canExecute?.Invoke(p) ?? true;
  }

  public void Execute(object? parameter)
  {
    if (parameter is not T p)
    {
      return;
    }

    _execute(p);
  }

  public event EventHandler? CanExecuteChanged;
  private readonly Func<T, bool>? _canExecute;
  private readonly Action<T> _execute;
}