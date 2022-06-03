using System;
using ICSharpCode.AvalonEdit;
using Orditor.Model;

namespace Orditor.Controls;

internal class Refactorings
{
  private readonly TextEditor _editor;

  public Refactorings(TextEditor editor)
  {
    _editor = editor;
  }

  public bool MoveUp()
  {
    var offset = _editor.CaretOffset;
    var line = _editor.Document.GetLineByOffset(offset);
    var previousLine = line.PreviousLine;
    if (previousLine == null)
    {
      return false;
    }

    var text = _editor.Document.GetText(line);

    var requirement = LineParser.TryRequirement(text);
    if (requirement == null)
    {
      return false;
    }

    var previousText = _editor.Document.GetText(previousLine);
    var previousRequirement = LineParser.TryRequirement(previousText);
    if (previousRequirement == null)
    {
      return false;
    }

    var lineSwappedText = text + Environment.NewLine + previousText;
    var newCaretOffset = previousLine.Offset + line.Length;
    _editor.Document.Replace(previousLine.Offset, line.EndOffset - previousLine.Offset, lineSwappedText);
    _editor.CaretOffset = newCaretOffset;
    return true;
  }

  public bool MoveDown()
  {
    var offset = _editor.CaretOffset;
    var line = _editor.Document.GetLineByOffset(offset);
    var nextLine = line.NextLine;
    if (nextLine == null)
    {
      return false;
    }

    var text = _editor.Document.GetText(line);

    var requirement = LineParser.TryRequirement(text);
    if (requirement == null)
    {
      return false;
    }

    var nextText = _editor.Document.GetText(nextLine);
    var nextRequirement = LineParser.TryRequirement(nextText);
    if (nextRequirement == null)
    {
      return false;
    }

    var lineSwappedText = nextText + Environment.NewLine + text;
    var newCaretOffset = nextLine.EndOffset;
    _editor.Document.Replace(line.Offset, nextLine.EndOffset - line.Offset, lineSwappedText);
    _editor.CaretOffset = newCaretOffset;
    return true;
  }
}