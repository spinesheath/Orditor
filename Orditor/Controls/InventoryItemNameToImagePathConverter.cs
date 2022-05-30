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
    { "casualcore", "/data/core.png" },
    { "casualdboost", "/data/dboost.png" },
    { "standardcore", "/data/core.png" },
    { "standarddboost", "/data/dboost.png" },
    { "standardlure", "/data/lure.png" },
    { "standardabilities", "/data/abilities.png" },
    { "expertcore", "/data/core.png" },
    { "expertdboost", "/data/dboost.png" },
    { "expertlure", "/data/lure.png" },
    { "expertabilities", "/data/abilities.png" },
    { "dbash", "/data/dbash.png" },
    { "mastercore", "/data/core.png" },
    { "masterdboost", "/data/dboost.png" },
    { "masterlure", "/data/lure.png" },
    { "masterabilities", "/data/abilities.png" },
    { "gjump", "/data/gjump.png" },
    { "glitched", "/data/glitched.png" },
    { "timedlevel", "/data/timedlevel.png" },
    { "insane", "/data/insane.png" },
    { "opendungeons", "/data/opendungeons.png" },
    { "openworld", "/data/openworld.png" }
  };
}