namespace Orditor.Model;

internal class Connection
{
  public Connection(string home, string target, Requirements requirement)
  {
    Home = home;
    Target = target;
    Requirement = requirement;
  }

  public string Home { get; }
  public Requirements Requirement { get; }
  public string Target { get; }

  public override string ToString()
  {
    return $"{Home} > {Target}: {Requirement}";
  }
}