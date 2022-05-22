using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;

namespace Orditor.Controls;

internal class PrefaceFolding : NewFolding
{
  public PrefaceFolding(ISegment line)
    : base(line.Offset, line.EndOffset)
  {
    Name = "preface";
  }
}