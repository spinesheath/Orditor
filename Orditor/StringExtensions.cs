using System.Collections.Generic;
using System.IO;

namespace Orditor;

internal static class StringExtensions
{
  public static IEnumerable<string> SplitToLines(this string text)
  {
    using var reader = new StringReader(text);
    while (reader.ReadLine() is { } line)
    {
      yield return line;
    }
  }
}