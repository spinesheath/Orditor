using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Folding;

namespace Orditor.Controls;

internal class CodeCompletion
{
  public CodeCompletion(TextEditor editor, FoldingManager foldingManager)
  {
    _editor = editor;
    _foldingManager = foldingManager;
    var textArea = editor.TextArea;
    textArea.TextEntering += TextEntering;
    textArea.TextEntered += TextEntered;
  }

  private static readonly List<string> Keywords = new()
  {
    "Free",
    "WallJump",
    "ChargeFlame",
    "DoubleJump",
    "Bash",
    "Stomp",
    "Glide",
    "Climb",
    "ChargeJump",
    "Grenade",
    "Dash",
    "Water",
    "Wind",
    "GinsoKey",
    "ForlornKey",
    "HoruKey",
    "TPGrove",
    "TPSwamp",
    "TPGrotto",
    "TPValley",
    "TPSorrow",
    "TPGinso",
    "TPForlorn",
    "TPHoru",
    "Mapstone",
    "OpenWorld",
    "Open"
  };

  private static readonly List<string> ConnectionSuggestions = new()
  {
    "pickup:",
    "conn:"
  };

  private static readonly List<string> LogicSuggestions = new()
  {
    "casual-core",
    "casual-dboost",
    "standard-core",
    "standard-dboost",
    "standard-lure",
    "standard-abilities",
    "expert-core",
    "expert-dboost",
    "expert-lure",
    "expert-abilities",
    "dbash",
    "master-core",
    "master-dboost",
    "master-lure",
    "expert-abilities",
    "gjump",
    "glitched",
    "timed-level",
    "insane",
  };

  private readonly TextEditor _editor;
  private readonly FoldingManager _foldingManager;
  private CompletionWindow? _completionWindow;

  private void TextEntered(object sender, TextCompositionEventArgs e)
  {
    var offset = _editor.CaretOffset;
    var line = _editor.Document.GetLineByOffset(offset);
    var previousLine = line.PreviousLine;
    if (previousLine == null)
    {
      return;
    }

    var foldings = _foldingManager.GetFoldingsContaining(offset);
    var isInsideHome = foldings.Any(f => f.Tag is HomeFolding);

    if (!isInsideHome)
    {
      return;
    }

    var text = _editor.Document.GetText(line);
    var indexInLine = offset - line.Offset;
    if (indexInLine < text.Length && !char.IsWhiteSpace(text[indexInLine]))
    {
      return;
    }

    var leadingTabs = -1;
    var previousWhitespace = -1;
    for (var i = 0; i < indexInLine; i++)
    {
      var c = text[i];
      if (c != '\t' && leadingTabs < 0)
      {
        leadingTabs = i;
      }

      if (char.IsWhiteSpace(c))
      {
        previousWhitespace = i;
      }
    }

    var partialText = text.Substring(previousWhitespace + 1, indexInLine - previousWhitespace - 1);
    
    if (leadingTabs == 1)
    {
      Show(ConnectionSuggestions, partialText);
    }
    else if (leadingTabs == 2)
    {
      Show(LogicSuggestions, partialText);
    }
  }

  private void Show(IEnumerable<string> keywords, string partialText)
  {
    var list = keywords.Select(keyword => new CompletionData(keyword, partialText)).ToList();
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