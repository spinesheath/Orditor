using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Orditor.ViewModels;

internal static class ObservableCollectionExtensions
{
  public static void Trim(this ObservableCollection<string> target, int afterIndex)
  {
    for (var i = target.Count - 1; i >= afterIndex; i--)
    {
      target.RemoveAt(i);
    }
  }

  public static void Update(this ObservableCollection<string> target, IEnumerable<string> newContent)
  {
    using var e = newContent.GetEnumerator();
    for (var i = 0; i < target.Count; i++)
    {
      if (e.MoveNext())
      {
        var oldItem = target[i];
        var newItem = e.Current;
        if (oldItem != newItem)
        {
          target.Insert(i, newItem);
        }
      }
      else
      {
        Trim(target, i);
        break;
      }
    }

    while (e.MoveNext())
    {
      target.Add(e.Current);
    }
  }
}