using System;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using Orditor.ViewModels;

namespace Orditor.Controls;

internal class CompletionData : NotificationObject, ICompletionData
{
  public CompletionData(string text)
  {
    Text = text;
  }

  public ImageSource? Image => null;

  public string Text { get; }
  
  public object Content => Text;

  public object Description => string.Empty;

  public double Priority { get; set; }

  public void Complete(TextArea textArea, ISegment segment, EventArgs insertionRequestEventArgs)
  {
    textArea.Document.Replace(segment, Text);
  }
}