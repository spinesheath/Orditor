using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Orditor.Controls;

internal class InventoryItemNameToImagePathConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    if (value is not string s)
    {
      return DependencyProperty.UnsetValue;
    }

    if (s.StartsWith("Tp"))
    {
      return "/Data/teleporter.png";
    }

    return Mapping.TryGetValue(s, out var path) ? path : $"/Data/{s.ToLowerInvariant()}.png";
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
    return DependencyProperty.UnsetValue;
  }

  private static readonly Dictionary<string, string> Mapping = new()
  {
    { "GinsoKey", "/Data/watervein.png" },
    { "ForlornKey", "/Data/gumonseal.png" },
    { "HoruKey", "/Data/sunstone.png" },
    { "Health", "/Data/hc.png" },
    { "Energy", "/Data/ec.png" },
    { "AbilityCells", "/Data/ac.png" },
    { "MapFragments", "/Data/ms.png" },
    { "Keystones", "/Data/ks.png" },
  };
}