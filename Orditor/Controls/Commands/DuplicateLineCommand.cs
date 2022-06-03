using System;
using System.Windows.Input;
using ICSharpCode.AvalonEdit;

namespace Orditor.Controls.Commands;

internal class DuplicateLineCommand : ICommand
{
  private readonly TextEditor _editor;

  public DuplicateLineCommand(TextEditor editor)
  {
    _editor = editor;
  }

  public bool CanExecute(object? parameter) => true;

  public void Execute(object? parameter)
  {
    var offset = _editor.CaretOffset;
    var line = _editor.Document.GetLineByOffset(offset);
    var text = _editor.Document.GetText(line);
    var toInsert = Environment.NewLine + text;
    _editor.Document.Insert(line.EndOffset, toInsert);
    _editor.CaretOffset = offset + toInsert.Length;
  }

  public event EventHandler? CanExecuteChanged { add { } remove { } }
}