namespace Orditor.Model;

internal class Connection
{
  public Connection(Home home, Location target, Requirements requirement)
  {
    Home = home;
    Target = target;
    Requirement = requirement;
  }

  public Home Home { get; }
  public Requirements Requirement { get; }
  public Location Target { get; }

  public override string ToString()
  {
    return $"{Home.Name} > {Target.Name}: {Requirement}";
  }
}