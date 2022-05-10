using System.Collections.Generic;
using System.Linq;

namespace Orditor.Model;

internal class Skill
{
  private Skill(string name, string id)
  {
    Name = name;
    Id = id;
  }

  public static Skill Bash { get; } = new("Bash", "SK0");
  public static Skill ChargeFlame { get; } = new("ChargeFlame", "SK2");
  public static Skill ChargeJump { get; } = new("ChargeJump", "SK8");
  public static Skill Climb { get; } = new("Climb", "SK12");
  public static Skill Dash { get; } = new("Dash", "SK50");
  public static Skill DoubleJump { get; } = new("DoubleJump", "SK5");
  public static Skill Glide { get; } = new("Glide", "SK14");
  public static Skill Grenade { get; } = new("Grenade", "SK51");
  public string Id { get; }
  public string Name { get; }
  public static Skill Stomp { get; } = new("Stomp", "SK4");
  public static Skill WallJump { get; } = new("WallJump", "SK3");

  public static Skill? Parse(string value)
  {
    return NameToSkill.TryGetValue(value, out var skill) ? skill : null;
  }

  public override string ToString()
  {
    return Name;
  }

  private static readonly IEnumerable<Skill> Skills = new[]
  {
    WallJump,
    ChargeFlame,
    DoubleJump,
    Bash,
    Stomp,
    Glide,
    Climb,
    ChargeJump,
    Dash,
    Grenade
  };

  private static readonly IReadOnlyDictionary<string, Skill> NameToSkill = Skills.ToDictionary(s => s.Name);
}