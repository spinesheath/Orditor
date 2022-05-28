using System;
using System.Reflection;
using Orditor.Model;

namespace Orditor.ViewModels;

internal class InventoryItemViewModel<T> : NotificationObject
{
  public InventoryItemViewModel(Inventory inventory, string propertyName)
  {
    _inventory = inventory;
    var propertyInfo = typeof(Inventory).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
    if (propertyInfo == null || propertyInfo.GetMethod == null || propertyInfo.SetMethod == null ||
        propertyInfo.PropertyType != typeof(T))
    {
      throw new ArgumentException($"No matching property found for {propertyName}.");
    }

    _property = propertyInfo;
    Name = propertyName;
  }

  public string Name { get; }

  public T Value
  {
    get => (T)_property.GetValue(_inventory)!;
    set => _property.SetValue(_inventory, value);
  }

  private readonly Inventory _inventory;
  private readonly PropertyInfo _property;
}