using System.Linq;

namespace Orditor.Controls;

internal class CompletionCandidate
{
  public CompletionCandidate(params string[] fragments)
  {
    Value = fragments[0];
    Fragments = fragments.Select(s => s.ToLowerInvariant()).ToArray();
  }

  public string Value { get; }

  public string[] Fragments { get; }
}