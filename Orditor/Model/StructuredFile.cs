using System.Collections.Generic;

namespace Orditor.Model;

internal class StructuredFile
{
  public void AddBlock(string name, string content)
  {
    _blockNameToIndex[name] = _blocks.Count;
    _blocks.Add(content);
  }

  public string GetBlock(string name)
  {
    return _blocks[_blockNameToIndex[name]];
  }

  private readonly Dictionary<string, int> _blockNameToIndex = new();

  private readonly List<string> _blocks = new();
}