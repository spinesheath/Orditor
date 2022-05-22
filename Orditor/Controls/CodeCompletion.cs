using System.Collections.Generic;
using System.Windows.Input;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace Orditor.Controls;

internal class CodeCompletion
{
  public CodeCompletion(TextEditor editor)
  {
    _editor = editor;
    var textArea = editor.TextArea;
    textArea.TextEntering += TextEntering;
    textArea.TextEntered += TextEntered;
  }

  private static readonly List<string> Keywords = new()
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

  private readonly TextEditor _editor;
  private CompletionWindow? _completionWindow;

  private void TextEntered(object sender, TextCompositionEventArgs e)
  {
    if (e.Text == ".")
    {
      _completionWindow = new CompletionWindow(_editor.TextArea);
      IList<ICompletionData> data = _completionWindow.CompletionList.CompletionData;
      foreach (var keyword in Keywords)
      {
        data.Add(new CompletionData(keyword));
      }

      _completionWindow.Show();
      _completionWindow.Closed += (_, _) => _completionWindow = null;
    }
  }

  private void TextEntering(object sender, TextCompositionEventArgs e)
  {
    if (e.Text.Length > 0 && _completionWindow != null)
    {
      if (!char.IsLetterOrDigit(e.Text[0]))
      {
        _completionWindow.CompletionList.RequestInsertion(e);
      }
    }
  }
}