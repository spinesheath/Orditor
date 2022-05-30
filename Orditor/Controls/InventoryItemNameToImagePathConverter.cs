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

    var lowercase = s.ToLowerInvariant();
    return Mapping.TryGetValue(lowercase, out var path) ? path : $"/Data/{lowercase}.png";
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
    return DependencyProperty.UnsetValue;
  }

  private static readonly Dictionary<string, string> Mapping = new()
  {
    { "ginsokey", "/Data/watervein.png" },
    { "forlornkey", "/Data/gumonseal.png" },
    { "horukey", "/Data/sunstone.png" },
    { "health", "/Data/hc.png" },
    { "energy", "/Data/ec.png" },
    { "abilitycells", "/Data/ac.png" },
    { "mapfragments", "/Data/ms.png" },
    { "keystones", "/Data/ks.png" },
    { "casualcore", "/data/ks.png" },
    { "casualdboost", "/data/ks.png" },
    { "standardcore", "/data/ks.png" },
    { "standarddboost", "/data/ks.png" },
    { "standardlure", "/data/ks.png" },
    { "standardabilities", "/data/ks.png" },
    { "expertcore", "/data/ks.png" },
    { "expertdboost", "/data/ks.png" },
    { "expertlure", "/data/ks.png" },
    { "expertabilities", "/data/ks.png" },
    { "dbash", "/data/ks.png" },
    { "mastercore", "/data/ks.png" },
    { "masterdboost", "/data/ks.png" },
    { "masterlure", "/data/ks.png" },
    { "masterabilities", "/data/ks.png" },
    { "gjump", "/data/ks.png" },
    { "glitched", "/data/ks.png" },
    { "timedlevel", "/data/ks.png" },
    { "insane", "/data/ks.png" }
  };
}