using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;

namespace Orditor.Controls;

internal class HomeFolding : NewFolding
{
  public HomeFolding(ISegment line, string name)
    : base(line.Offset, line.EndOffset)
  {
    Name = name;
  }
}