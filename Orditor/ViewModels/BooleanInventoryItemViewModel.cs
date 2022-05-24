using System;
using System.Reflection;
using Orditor.Model;

namespace Orditor.ViewModels;

internal class BooleanInventoryItemViewModel : NotificationObject
{
  public BooleanInventoryItemViewModel(Inventory inventory, string propertyName)
  {
    _inventory = inventory;
    var propertyInfo = typeof(Inventory).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
    if (propertyInfo == null || propertyInfo.GetMethod == null || propertyInfo.SetMethod == null ||
        propertyInfo.PropertyType != typeof(bool))
    {
      throw new ArgumentException($"No matching property found for {propertyName}.");
    }

    _property = propertyInfo;
    Name = propertyName;
  }

  public bool Value
  {
    get => (bool)_property.GetValue(_inventory)!;
    set => _property.SetValue(_inventory, value);
  }

  public string Name { get; }

  private readonly Inventory _inventory;
  private readonly PropertyInfo _property;
}