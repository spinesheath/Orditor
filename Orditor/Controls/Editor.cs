using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;

namespace Orditor.Controls;

internal class FoldingStrategy
{
  private readonly FoldingManager _foldingManager;

  public FoldingStrategy(FoldingManager foldingManager)
  {
    _foldingManager = foldingManager;
  }

  public void UpdateFoldings(TextDocument document)
  {
    var foldings = SafeCreateNewFoldings(document, out var firstErrorOffset);
    _foldingManager.UpdateFoldings(foldings, firstErrorOffset);
  }

  public IEnumerable<NewFolding> SafeCreateNewFoldings(TextDocument document, out int firstErrorOffset)
  {
    try
    {
      return CreateNewFoldings(document, out firstErrorOffset);
    }
    catch
    {
      firstErrorOffset = 0;
      return Enumerable.Empty<NewFolding>();
    }
  }

  public IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
  {
    var stack = new Stack<NewFolding>();
    var foldMarkers = new List<NewFolding>();
    var index = 0;

    if (document.Lines.Count < 2)
    {
      firstErrorOffset = -1;
      return Enumerable.Empty<NewFolding>();
    }

    try
    {
      var prefaceFolding = new NewFolding(0, 0);
      prefaceFolding.Name = "preface";
      stack.Push(prefaceFolding);
      var lines = document.Lines;
      var previousLine = lines[0];
      for (; index < lines.Count; index++)
      {
        var line = lines[index];
        var trimmed = document.GetText(line).Trim();
        if (trimmed.StartsWith("home"))
        {
          var previous = stack.Pop();
          previous.EndOffset = previousLine.EndOffset;
          foldMarkers.Add(previous);
          
          var homeFolding = new NewFolding(line.Offset, line.EndOffset);
          homeFolding.Name = trimmed;
          stack.Push(homeFolding);
        }

        previousLine = line;
      }

      firstErrorOffset = -1;
    }
    catch
    {
      if (index >= 1 && index <= document.LineCount)
        firstErrorOffset = document.GetOffset(index, 0);
      else
        firstErrorOffset = 0;
    }

    foldMarkers.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
    return foldMarkers;
  }
}

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