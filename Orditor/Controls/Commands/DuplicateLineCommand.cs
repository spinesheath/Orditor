using System;
using ICSharpCode.AvalonEdit;

namespace Orditor.Controls.Commands;

internal class DuplicateLineCommand : BaseCommand
{
  public DuplicateLineCommand(TextEditor editor)
  {
    _editor = editor;
  }

  protected override void Execute()
  {
    var offset = _editor.CaretOffset;
    var line = _editor.Document.GetLineByOffset(offset);
    var text = _editor.Document.GetText(line);
    var toInsert = Environment.NewLine + text;
    _editor.Document.Insert(line.EndOffset, toInsert);
    _editor.CaretOffset = offset + toInsert.Length;
  }

  private readonly TextEditor _editor;
}