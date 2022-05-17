using System;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Folding;

namespace Orditor.Controls;

internal class Editor : Decorator
{
  public Editor()
  {
    Initialized += SetChild;
    _textEditor.TextChanged += OnTextChangedInternal;
    var foldingManager = FoldingManager.Install(_textEditor.TextArea);
    _foldingStrategy = new FoldingStrategy(foldingManager);
  }

  public int FocusedLineIndex
  {
    get => (int)GetValue(FocusedLineIndexProperty);
    set => SetValue(FocusedLineIndexProperty, value);
  }

  public string Text
  {
    get => (string)GetValue(TextProperty);
    set => SetValue(TextProperty, value);
  }

  public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
    nameof(Text), typeof(string), typeof(Editor), TextMetadata());

  public static readonly DependencyProperty FocusedLineIndexProperty = DependencyProperty.Register(
    nameof(FocusedLineIndex), typeof(int), typeof(Editor), new PropertyMetadata(default(int), OnLineIndexChanged));

  private readonly TextEditor _textEditor = new();
  private readonly FoldingStrategy _foldingStrategy;

  private static void OnLineIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    var editor = (Editor)d;
    editor.OnLineIndexChanged();
  }

  private void OnLineIndexChanged()
  {
    _textEditor.ScrollTo(FocusedLineIndex, 0);
  }

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
      _foldingStrategy.UpdateFoldings(_textEditor.Document);
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