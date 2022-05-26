namespace Orditor.Model;

internal class Home : Location
{
  public Home(string name, int id, int x, int y)
    : base(name, id, x, y)
  {
  }
  
  public void SetLocation(int x, int y)
  {
    X = x;
    Y = y;
  }
}