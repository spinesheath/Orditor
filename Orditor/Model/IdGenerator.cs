namespace Orditor.Model;

internal class IdGenerator
{
  private int _next;

  public int Next()
  {
    return _next++;
  }
}