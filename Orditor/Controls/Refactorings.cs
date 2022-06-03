using System;
using System.Collections.Generic;
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

  public bool MoveLeft()
  {
    var offset = _editor.CaretOffset;
    var line = _editor.Document.GetLineByOffset(offset);
    var text = _editor.Document.GetText(line);

    var requirement = LineParser.TryRequirement(text);
    if (requirement == null)
    {
      return false;
    }

    var withoutComments = LineParser.StripComment(text);
    var offsetInLine = offset - line.Offset;
    if (offsetInLine >= withoutComments.Length)
    {
      return false;
    }

    var tokens = new List<string>();
    var tokenOffsets = new List<int>();
    var currentlyWhitespace = char.IsWhiteSpace(text[0]);
    var tokenStartIndex = 0;
    var tokenIndexAtCaret = 0;
    for (var i = 1; i < withoutComments.Length; i++)
    {
      if (i == offsetInLine)
      {
        tokenIndexAtCaret = tokens.Count;
      }

      if (char.IsWhiteSpace(text[i]) != currentlyWhitespace)
      {
        currentlyWhitespace = !currentlyWhitespace;
        tokens.Add(text.Substring(tokenStartIndex, i - tokenStartIndex));
        tokenOffsets.Add(tokenStartIndex);
        tokenStartIndex = i;
      }
    }

    if (tokenIndexAtCaret < 5 || string.IsNullOrWhiteSpace(tokens[tokenIndexAtCaret]))
    {
      return false;
    }

    var replacement = tokens[tokenIndexAtCaret] + tokens[tokenIndexAtCaret - 1] + tokens[tokenIndexAtCaret - 2];
    var replacementStartOffset = line.Offset + tokenOffsets[tokenIndexAtCaret - 2];
    _editor.Document.Replace(replacementStartOffset, replacement.Length, replacement);
    _editor.CaretOffset = replacementStartOffset + offsetInLine - tokenOffsets[tokenIndexAtCaret];

    return true;
  }

  public bool MoveRight()
  {
    var offset = _editor.CaretOffset;
    var line = _editor.Document.GetLineByOffset(offset);
    var text = _editor.Document.GetText(line);

    var requirement = LineParser.TryRequirement(text);
    if (requirement == null)
    {
      return false;
    }

    var withoutComments = LineParser.StripComment(text);
    var offsetInLine = offset - line.Offset;
    if (offsetInLine >= withoutComments.Length)
    {
      return false;
    }

    var tokens = new List<string>();
    var tokenOffsets = new List<int>();
    var currentlyWhitespace = char.IsWhiteSpace(text[0]);
    var tokenStartIndex = 0;
    var tokenIndexAtCaret = 0;
    for (var i = 1; i < withoutComments.Length; i++)
    {
      if (i == offsetInLine)
      {
        tokenIndexAtCaret = tokens.Count;
      }

      if (char.IsWhiteSpace(text[i]) != currentlyWhitespace)
      {
        currentlyWhitespace = !currentlyWhitespace;
        tokens.Add(text.Substring(tokenStartIndex, i - tokenStartIndex));
        tokenOffsets.Add(tokenStartIndex);
        tokenStartIndex = i;
      }
    }

    if (tokenIndexAtCaret < 3 || tokenIndexAtCaret >= tokens.Count - 2 || string.IsNullOrWhiteSpace(tokens[tokenIndexAtCaret]))
    {
      return false;
    }

    var replacement = tokens[tokenIndexAtCaret + 2] + tokens[tokenIndexAtCaret + 1] + tokens[tokenIndexAtCaret];
    var replacementStartOffset = line.Offset + tokenOffsets[tokenIndexAtCaret];
    _editor.Document.Replace(replacementStartOffset, replacement.Length, replacement);
    _editor.CaretOffset = offset + replacement.Length - tokens[tokenIndexAtCaret].Length;

    return true;
  }
}