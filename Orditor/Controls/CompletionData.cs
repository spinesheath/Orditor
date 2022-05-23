using System;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using Orditor.ViewModels;

namespace Orditor.Controls;

internal class CompletionData : NotificationObject, ICompletionData
{
  public CompletionData(string text, string partialText)
  {
    _partialText = partialText;
    Text = text;
    Priority = CalculatePriority(text, partialText);
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

  private static int CalculatePriority(string text, string partialText)
  {
    // TODO if partial="dj" then prioritize DoubleJump etc.
    var i = 0;
    for (; i < text.Length && i < partialText.Length && text[i] == partialText[i]; i++) { }
    return i;
  }
}