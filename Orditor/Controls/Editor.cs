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
    _textEditor.TextChanged += OnTextChangedInternal;
  }

  public string Text
  {
    get => (string)GetValue(TextProperty);
    set => SetValue(TextProperty, value);
  }

  public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
    nameof(Text), typeof(string), typeof(Editor), TextMetadata());

  private readonly TextEditor _textEditor = new();

  private static FrameworkPropertyMetadata TextMetadata()
  {
    return new FrameworkPropertyMetadata(default(string), OnTextChanged) { BindsTwoWayByDefault = true };
  }

  private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    ((Editor)d).OnTextChanged();
  }

  private void OnTextChanged()
  {
    if (_textEditor.Text != Text)
    {
      _textEditor.Text = Text;
    }
  }

  private void SetChild(object? sender, EventArgs e)
  {
    Child = _textEditor;
  }

  private void OnTextChangedInternal(object? sender, EventArgs e)
  {
    Text = _textEditor.Text;
  }
}