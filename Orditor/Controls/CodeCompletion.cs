﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using NLog;
using Orditor.Model;

namespace Orditor.Controls;

internal class CodeCompletion
{
  private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

  public CodeCompletion(TextEditor editor)
  {
    _editor = editor;
    var textArea = editor.TextArea;
    textArea.TextEntering += TextEntering;
    textArea.TextEntered += TextEntered;
  }

  private readonly TextEditor _editor;
  private CompletionWindow? _completionWindow;

  private void TextEntered(object sender, TextCompositionEventArgs e)
  {
    try
    {
      PrepareSuggestions(e);
    }
    catch (Exception exception)
    {
      Logger.Error(exception);
    }
  }

  private void PrepareSuggestions(TextCompositionEventArgs e)
  {
    if (string.IsNullOrWhiteSpace(e.TextComposition.Text))
    {
      return;
    }

    var caretOffset = _editor.CaretOffset;
    var currentLine = _editor.Document.GetLineByOffset(caretOffset);

    if (!IsInsideHome(currentLine))
    {
      return;
    }

    var text = _editor.Document.GetText(currentLine);
    var indexInLine = Math.Min(caretOffset - currentLine.Offset, text.Length);
    if (indexInLine < text.Length && !char.IsWhiteSpace(text[indexInLine]))
    {
      return;
    }

    if (indexInLine > 0 && char.IsDigit(text[indexInLine - 1]))
    {
      Hide();
      return;
    }

    var leadingTabs = -1;
    var previousWhitespaceIndex = -1;
    var segmentIndex = 0;
    for (var i = 0; i < indexInLine; i++)
    {
      var c = text[i];
      if (c == '-' && text.Length > i + 1 && text[i + 1] == '-')
      {
        Hide();
        return;
      }

      if (c != '\t' && leadingTabs < 0)
      {
        leadingTabs = i;
      }

      if (char.IsWhiteSpace(c))
      {
        previousWhitespaceIndex = i;
      }
      else
      {
        if (previousWhitespaceIndex + 1 == i)
        {
          segmentIndex += 1;
        }
      }
    }

    var partialText = text.Substring(previousWhitespaceIndex + 1, indexInLine - previousWhitespaceIndex - 1);

    if (leadingTabs == 2 && segmentIndex == 1)
    {
      Show(CompletionCandidates.Logic, partialText);
    }
    else if (leadingTabs == 2 && segmentIndex > 1)
    {
      Show(CompletionCandidates.Requirements, partialText);
    }
    else if (leadingTabs == 1 && segmentIndex == 1)
    {
      if (partialText[^1] == ':')
      {
        Hide();
      }
      else if (string.IsNullOrWhiteSpace(text[indexInLine..]))
      {
        Show(CompletionCandidates.Connection, partialText);
      }
    }
    else if (leadingTabs == 1 && segmentIndex > 1)
    {
      if (text.Substring(1, 5) == "conn:" && string.IsNullOrWhiteSpace(text[indexInLine..]))
      {
        Show(Homes(), partialText);
      }
      else if (text.Substring(1, 7) == "pickup:" && string.IsNullOrWhiteSpace(text[indexInLine..]))
      {
        Show(Pickups(), partialText);
      }
    }
  }

  private void Hide()
  {
    _completionWindow?.Close();
  }

  private IEnumerable<CompletionCandidate> Pickups()
  {
    foreach (var line in _editor.Document.Lines)
    {
      if (LineParser.TryPickupName(_editor.Document.GetText(line)) is { } name)
      {
        yield return new CompletionCandidate(name);
      }
    }
  }

  private IEnumerable<CompletionCandidate> Homes()
  {
    foreach (var line in _editor.Document.Lines)
    {
      if (LineParser.TryHomeName(_editor.Document.GetText(line)) is { } name)
      {
        yield return new CompletionCandidate(name);
      }
    }
  }

  private bool IsInsideHome(DocumentLine line)
  {
    if (LineParser.IsHome(_editor.Document.GetText(line)))
    {
      return false;
    }

    var currentLine = line.PreviousLine;
    while (currentLine != null)
    {
      var text = _editor.Document.GetText(currentLine);
      if (LineParser.IsHome(text))
      {
        return true;
      }

      currentLine = currentLine.PreviousLine;
    }

    return false;
  }

  private void Show(IEnumerable<CompletionCandidate> candidates, string partialText)
  {
    var lowercase = partialText.ToLowerInvariant();
    var list = candidates.Select(c => new CompletionData(c, lowercase)).ToList();
    if (list.Count == 0)
    {
      return;
    }

    list.Sort((x, y) => y.Priority.CompareTo(x.Priority));

    _completionWindow = new CompletionWindow(_editor.TextArea);
    var data = _completionWindow.CompletionList.CompletionData;
    foreach (var item in list)
    {
      data.Add(item);
    }

    _completionWindow.CompletionList.IsFiltering = false;
    _completionWindow.CompletionList.SelectedItem = list[0];

    _completionWindow.Show();
    _completionWindow.Closed += (_, _) => _completionWindow = null;
  }

  private void TextEntering(object sender, TextCompositionEventArgs e)
  {
    if (e.Text.Length > 0 && _completionWindow != null)
    {
      if (e.Text[0] == '\r' || e.Text[0] == '\n' || e.Text[0] == ' ')
      {
        _completionWindow.CompletionList.RequestInsertion(e);
      }
    }
  }
}