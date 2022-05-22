using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;

namespace Orditor.Controls;

internal class PickupFolding : NewFolding
{
  public PickupFolding(ISegment line, string name)
    : base(line.Offset, line.EndOffset)
  {
    Name = name;
  }
}