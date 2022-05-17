using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;

namespace Orditor.Controls;

internal class FoldingStrategy
{
  public FoldingStrategy(FoldingManager foldingManager)
  {
    _foldingManager = foldingManager;
  }

  public int FoldAllBut(string home)
  {
    var unfoldedStartOffset = int.MaxValue;
    foreach (var folding in _foldingManager.AllFoldings)
    {
      var isHome = folding.Title.Contains("home");
      var fold = folding.Title.Contains("preface") || isHome && !folding.Title.Contains(home);
      folding.IsFolded = fold;
      if (isHome && !fold)
      {
        unfoldedStartOffset = Math.Min(unfoldedStartOffset, folding.StartOffset);
      }
    }

    return unfoldedStartOffset;
  }

  public void UpdateFoldings(TextDocument document)
  {
    var foldings = SafeCreateNewFoldings(document, out var firstErrorOffset);
    _foldingManager.UpdateFoldings(foldings, firstErrorOffset);
  }

  private readonly FoldingManager _foldingManager;

  private IEnumerable<NewFolding> SafeCreateNewFoldings(TextDocument document, out int firstErrorOffset)
  {
    try
    {
      return CreateNewFoldings(document, out firstErrorOffset);
    }
    catch
    {
      firstErrorOffset = 0;
      return Array.Empty<NewFolding>();
    }
  }

  private IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
  {
    var candidates = new Stack<NewFolding>();
    var result = new List<NewFolding>();
    var index = 0;

    if (document.Lines.Count < 2)
    {
      firstErrorOffset = -1;
      return Array.Empty<NewFolding>();
    }

    try
    {
      var lines = document.Lines;
      var previousLine = lines[0];
      candidates.Push(new OtherFolding(previousLine, "preface"));
      for (; index < lines.Count; index++)
      {
        var line = lines[index];
        var trimmed = document.GetText(line).Trim();
        if (trimmed.StartsWith("home"))
        {
          while (candidates.Count > 0)
          {
            PopAndAdd(candidates, result, previousLine.EndOffset);
          }

          candidates.Push(new HomeFolding(line, trimmed));
        }

        if (trimmed.StartsWith("pickup") || trimmed.StartsWith("conn"))
        {
          if (candidates.Peek() is OtherFolding)
          {
            PopAndAdd(candidates, result, previousLine.EndOffset);
          }

          candidates.Push(new OtherFolding(line, "    " + trimmed));
        }

        previousLine = line;
      }

      while (candidates.Count > 0)
      {
        PopAndAdd(candidates, result, previousLine.EndOffset);
      }

      firstErrorOffset = -1;
    }
    catch
    {
      if (index >= 1 && index <= document.LineCount)
      {
        firstErrorOffset = document.GetOffset(index, 0);
      }
      else
      {
        firstErrorOffset = 0;
      }
    }

    result.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
    return result;
  }

  private static void PopAndAdd(Stack<NewFolding> candidates, ICollection<NewFolding> foldings, int endOffset)
  {
    var previous = candidates.Pop();
    if (previous.EndOffset == endOffset)
    {
      return;
    }

    previous.EndOffset = endOffset;
    foldings.Add(previous);
  }

  private class HomeFolding : NewFolding
  {
    public HomeFolding(ISegment line, string name)
      : base(line.Offset, line.EndOffset)
    {
      Name = name;
    }
  }

  private class OtherFolding : NewFolding
  {
    public OtherFolding(ISegment line, string name)
      : base(line.Offset, line.EndOffset)
    {
      Name = name;
    }
  }
}