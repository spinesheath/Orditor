using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Search;
using Orditor.Controls.Commands;
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
    _textEditor.SyntaxHighlighting = LoadHighlighting();
    _ = new CodeCompletion(_textEditor);
    _ = SearchPanel.Install(_textEditor);
    _refactorings = new Refactorings(_textEditor);
    _textEditor.InputBindings.Add(InputBindingFactory.DuplicateLine(_textEditor));
  }

  public Messenger Messenger
  {
    get => (Messenger)GetValue(MessengerProperty);
    set => SetValue(MessengerProperty, value);
  }

  public string Text
  {
    get => (string)GetValue(TextProperty);
    set => SetValue(TextProperty, value);
  }

  public void Selected(Home home)
  {
    var unfoldedOffset = _foldingStrategy.FoldAllBut(home);
    ScrollTo(unfoldedOffset);
  }

  public void Selected(Pickup pickup)
  {
    var unfoldedOffset = _foldingStrategy.FoldAllBut(_textEditor.Document, pickup);
    ScrollTo(unfoldedOffset);
  }

  public void Selected(Home home1, Home home2)
  {
    var unfoldedOffset = _foldingStrategy.FoldAllBut(home1, home2);
    ScrollTo(unfoldedOffset);
  }

  public void Selected(Home home, Pickup pickup)
  {
    var unfoldedOffset = _foldingStrategy.FoldAllBut(home, pickup);
    ScrollTo(unfoldedOffset);
  }

  private const string SyntaxResourceName = "Orditor.Data.areasSyntax.xml";

  public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
    nameof(Text), typeof(string), typeof(Editor), TextMetadata());

  public static readonly DependencyProperty MessengerProperty = DependencyProperty.Register(
    nameof(Messenger), typeof(Messenger), typeof(Editor), new PropertyMetadata(default(Messenger), OnSelectionChanged));

  private readonly FoldingStrategy _foldingStrategy;

  private readonly TextEditor _textEditor = new();
  private readonly Refactorings _refactorings;

  protected override void OnPreviewKeyDown(KeyEventArgs e)
  {
    if (e.Key != Key.Up && e.Key != Key.Down)
    {
      _foldingStrategy.Unfold(_textEditor.CaretOffset);
    }

    var ctrlAltShift = ModifierKeys.Control | ModifierKeys.Alt | ModifierKeys.Shift;
    if ((Keyboard.Modifiers & ctrlAltShift) == ctrlAltShift)
    {
      if (e.Key == Key.Up && _refactorings.MoveUp())
      {
        e.Handled = true;
      }
      else if (e.Key == Key.Down && _refactorings.MoveDown())
      {
        e.Handled = true;
      }
      else if (e.Key == Key.Left && _refactorings.MoveLeft())
      {
        e.Handled = true;
      }
      else if (e.Key == Key.Right && _refactorings.MoveRight())
      {
        e.Handled = true;
      }
    }

    base.OnKeyDown(e);
  }

  private static IHighlightingDefinition LoadHighlighting()
  {
    var assembly = typeof(Annotations).Assembly;
    using var stream = assembly.GetManifestResourceStream(SyntaxResourceName);
    using var reader = new XmlTextReader(stream!);
    var resolver = new HighlightingManager();
    return HighlightingLoader.Load(reader, resolver);
  }

  private void ScrollTo(int unfoldedOffset)
  {
    if (unfoldedOffset < 0 || unfoldedOffset >= _textEditor.Document.TextLength)
    {
      return;
    }

    var line = _textEditor.Document.GetLineByOffset(unfoldedOffset);
    _textEditor.ScrollTo(line.LineNumber, 0);
    _textEditor.CaretOffset = unfoldedOffset;
    _textEditor.Focus();
  }

  private static void OnSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    var editor = (Editor)d;
    if (e.OldValue is Messenger oldSelection)
    {
      oldSelection.StopListening(editor);
    }

    if (e.NewValue is Messenger newSelection)
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
}