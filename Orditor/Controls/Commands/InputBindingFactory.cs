using System.Windows.Input;
using ICSharpCode.AvalonEdit;

namespace Orditor.Controls.Commands;

internal static class InputBindingFactory
{
  public static InputBinding DuplicateLine(TextEditor editor)
  {
    return new InputBinding(new DuplicateLineCommand(editor), new KeyGesture(Key.D, ModifierKeys.Control | ModifierKeys.Shift));
  }
}