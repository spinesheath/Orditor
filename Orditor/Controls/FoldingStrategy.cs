using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using Orditor.Model;

namespace Orditor.Controls;

internal class FoldingStrategy
{
  public FoldingStrategy(FoldingManager foldingManager)
  {
    _foldingManager = foldingManager;
  }

  public int FoldAllBut(Home home)
  {
    var unfoldedStartOffset = int.MaxValue;
    foreach (var folding in _foldingManager.AllFoldings)
    {
      var possibleHome = LineParser.TryHome(folding.Title);
      var isHome = possibleHome != null;
      var fold = IsPreface(folding) || (isHome && home.Name != possibleHome);
      folding.IsFolded = fold;
      if (isHome && !fold)
      {
        unfoldedStartOffset = Math.Min(unfoldedStartOffset, folding.StartOffset);
      }
    }

    return unfoldedStartOffset;
  }

  public int FoldAllBut(Home home1, Home home2)
  {
    var unfoldedStartOffset = int.MaxValue;
    foreach (var folding in _foldingManager.AllFoldings)
    {
      var possibleHome = LineParser.TryHome(folding.Title);
      var isHome = possibleHome != null;
      var fold = IsPreface(folding) || (isHome && possibleHome != home1.Name && possibleHome != home2.Name);
      folding.IsFolded = fold;
      if (isHome && !fold)
      {
        unfoldedStartOffset = Math.Min(unfoldedStartOffset, folding.StartOffset);
      }
    }

    return unfoldedStartOffset;
  }

  public int FoldAllBut(TextDocument document, Pickup pickup)
  {
    var pickupOffsets = GetPickupOffsets(document, pickup);

    foreach (var folding in _foldingManager.AllFoldings)
    {
      folding.IsFolded = !pickupOffsets.Any(offset => offset >= folding.StartOffset && offset <= folding.EndOffset);
    }

    return pickupOffsets.FirstOrDefault();
  }

  public int FoldAllBut(TextDocument document, Home home, Pickup pickup)
  {
    var pickupOffsets = GetPickupOffsets(document, pickup);

    foreach (var folding in _foldingManager.AllFoldings)
    {
      if (Is(folding, home))
      {
        folding.IsFolded = false;
      }
      else if (Is(folding, pickup) && _foldingManager.GetFoldingsContaining(folding.StartOffset).Any(f => Is(f, home)))
      {
        folding.IsFolded = false;
      }
      else
      {
        folding.IsFolded = true;
      }
    }

    return pickupOffsets.FirstOrDefault();
  }

  public void UpdateFoldings(TextDocument document)
  {
    var foldings = SafeCreateNewFoldings(document, out var firstErrorOffset);
    _foldingManager.UpdateFoldings(foldings, firstErrorOffset);
  }

  private const string Preface = "preface";

  private readonly FoldingManager _foldingManager;

  private static bool IsPreface(FoldingSection folding)
  {
    return folding.Title == Preface;
  }

  private static List<int> GetPickupOffsets(TextDocument document, Pickup pickup)
  {
    var pickupOffsets = new List<int>();
    foreach (var line in document.Lines)
    {
      var possiblePickup = LineParser.TryPickupReference(document.GetText(line));
      if (possiblePickup == pickup.Name)
      {
        pickupOffsets.Add(line.Offset);
      }
    }

    return pickupOffsets;
  }

  private static bool Is(FoldingSection folding, Pickup pickup)
  {
    return LineParser.TryPickupReference(folding.Title) == pickup.Name;
  }

  private static bool Is(FoldingSection folding, Home home)
  {
    return LineParser.TryHome(folding.Title) == home.Name;
  }

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
      candidates.Push(new OtherFolding(previousLine, Preface));
      for (; index < lines.Count; index++)
      {
        var line = lines[index];
        var text = document.GetText(line);
        if (LineParser.IsHome(text))
        {
          while (candidates.Count > 0)
          {
            PopAndAdd(candidates, result, previousLine.EndOffset);
          }

          candidates.Push(new HomeFolding(line, text));
        }

        if (LineParser.IsPickupReference(text) || LineParser.IsConnection(text))
        {
          if (candidates.Peek() is OtherFolding)
          {
            PopAndAdd(candidates, result, previousLine.EndOffset);
          }

          candidates.Push(new OtherFolding(line, "    " + text.Trim()));
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