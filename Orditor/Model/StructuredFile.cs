using System.Collections.Generic;

namespace Orditor.Model;

internal class StructuredFile
{
  public StructuredFile(string content)
  {
    Content = content;
  }

  public string Content { get; }

  public void AddBlock(string name, int lineIndex)
  {
    _blockNameToLineIndex[name] = lineIndex;
  }

  public int LineIndex(string blockName)
  {
    return _blockNameToLineIndex[blockName];
  }

  private readonly Dictionary<string, int> _blockNameToLineIndex = new();
}