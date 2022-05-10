﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Orditor.ViewModels;

internal class NotificationObject : INotifyPropertyChanged
{
  public event PropertyChangedEventHandler? PropertyChanged;

  protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
  {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
}