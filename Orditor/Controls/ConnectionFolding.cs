using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;

namespace Orditor.Controls;

internal class ConnectionFolding : NewFolding
{
  public ConnectionFolding(ISegment line, string name)
    : base(line.Offset, line.EndOffset)
  {
    Name = name;
  }
}