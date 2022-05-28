using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Orditor.Controls;

internal class InventoryItemNameToImagePathConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    if (value is string s)
    {
      return $"/Data/{s.ToLowerInvariant()}.png";
    }

    return DependencyProperty.UnsetValue;
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
    return DependencyProperty.UnsetValue;
  }
}