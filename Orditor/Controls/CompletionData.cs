using System;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using Orditor.ViewModels;

namespace Orditor.Controls;

internal class CompletionData : NotificationObject, ICompletionData
{
  public CompletionData(CompletionCandidate candidate, string partialText)
  {
    _partialText = partialText;
    Text = candidate.Value;
    Priority = CalculatePriority(candidate, partialText);
  }

  public ImageSource? Image => null;

  public string Text { get; }

  public object Content => Text;

  public object? Description => null;

  public double Priority { get; }

  public void Complete(TextArea textArea, ISegment segment, EventArgs insertionRequestEventArgs)
  {
    textArea.Document.Replace(segment.Offset - _partialText.Length, _partialText.Length, Text);
  }

  private readonly string _partialText;

  private static int CalculatePriority(CompletionCandidate candidate, string partialText)
  {
    var best = 0;
    foreach (var fragment in candidate.Fragments)
    {
      var i = 0;
      for (; i < fragment.Length && i < partialText.Length && fragment[i] == partialText[i]; i++) { }

      best = i == fragment.Length ? 1000 : Math.Max(best, i);
    }

    return best;
  }
}