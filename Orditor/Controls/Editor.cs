using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Folding;
using Orditor.Model;
using Orditor.Orchestration;

namespace Orditor.Controls;

internal class Editor : Decorator, ISelectionListener
{
  public Editor()
  {
    Initialized += SetChild;
    _textEditor.FontFamily = new FontFamily("Consolas");
    _textEditor.ShowLineNumbers = true;
    _textEditor.TextChanged += OnTextChangedInternal;
    var foldingManager = FoldingManager.Install(_textEditor.TextArea);
    _foldingStrategy = new FoldingStrategy(foldingManager);
  }

  public Selection Selection
  {
    get => (Selection)GetValue(SelectionProperty);
    set => SetValue(SelectionProperty, value);
  }

  public string Text
  {
    get => (string)GetValue(TextProperty);
    set => SetValue(TextProperty, value);
  }

  public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
    nameof(Text), typeof(string), typeof(Editor), TextMetadata());

  public static readonly DependencyProperty SelectionProperty = DependencyProperty.Register(
    nameof(Selection), typeof(Selection), typeof(Editor), new PropertyMetadata(default(Selection), OnSelectionChanged));

  private readonly FoldingStrategy _foldingStrategy;

  private readonly TextEditor _textEditor = new();

  private static void OnSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    var editor = (Editor)d;
    if (e.OldValue is Selection oldSelection)
    {
      oldSelection.StopListening(editor);
    }

    if (e.NewValue is Selection newSelection)
    {
      newSelection.Listen(editor);
    }
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

  public void Selected(Home home)
  {
    var unfoldedOffset = _foldingStrategy.FoldAllBut(home);
    var line = _textEditor.Document.GetLineByOffset(unfoldedOffset);
    _textEditor.ScrollTo(line.LineNumber, 0);
  }

  public void Selected(Pickup pickup)
  {
    var unfoldedOffset = _foldingStrategy.FoldAllBut(_textEditor.Document, pickup);
    var line = _textEditor.Document.GetLineByOffset(unfoldedOffset);
    _textEditor.ScrollTo(line.LineNumber, 0);
  }

  public void Selected(Home home1, Home home2)
  {
    var unfoldedOffset = _foldingStrategy.FoldAllBut(home1, home2);
    var line = _textEditor.Document.GetLineByOffset(unfoldedOffset);
    _textEditor.ScrollTo(line.LineNumber, 0);
  }

  public void Selected(Home home1, Pickup pickup) { }
}