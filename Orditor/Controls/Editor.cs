using System;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit;

namespace Orditor.Controls;

internal class Editor : Decorator
{
  public Editor()
  {
    Initialized += SetChild;
  }

  public string Text
  {
    get => (string)GetValue(TextProperty);
    set => SetValue(TextProperty, value);
  }

  public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
    nameof(Text), typeof(string), typeof(Editor), new PropertyMetadata(default(string), OnTextChanged));

  private readonly TextEditor _textEditor = new();

  private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    ((Editor)d).OnTextChanged();
  }

  private void OnTextChanged()
  {
    _textEditor.Text = Text;
  }

  private void SetChild(object? sender, EventArgs e)
  {
    Child = _textEditor;
  }
}