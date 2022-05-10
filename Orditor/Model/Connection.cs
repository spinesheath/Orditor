namespace Orditor.Model;

internal class Connection
{
  public Connection(string home, string target, Requirements requirement, bool isPickup)
  {
    Home = home;
    Target = target;
    Requirement = requirement;
    IsPickup = isPickup;
  }

  public string Home { get; }
  public bool IsPickup { get; }
  public Requirements Requirement { get; }
  public string Target { get; }

  public override string ToString()
  {
    return $"{Home} > {Target}: {Requirement}";
  }
}