using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orditor.Model;

internal class Requirements
{
  public Requirements(string raw)
  {
    var parts = raw.Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries);
    Logic = parts[0];

    var skills = new List<Skill>();
    var other = new List<string>();

    foreach (var part in parts.Skip(1))
    {
      var p = part.Split('=');
      if (p.Length == 1)
      {
        var skill = Skill.Parse(part);
        if (skill != null)
        {
          skills.Add(skill);
        }
        else
        {
          other.Add(part);
        }
      }
      else if (p.Length == 2)
      {
        var count = ParsePositiveInteger(p[1]);
        if (p[0] == "Health")
        {
          Health += count;
        }
        else if (p[0] == "Energy")
        {
          Energy += count;
        }
        else if (p[0] == "Ability")
        {
          Ability += count;
        }
        else if (p[0] == "Keystone")
        {
          Keystone += count;
        }
      }
    }

    Skills = skills;
    Other = other;
  }

  private static int ParsePositiveInteger(string value)
  {
    return int.TryParse(value, out var n) ? Math.Max(n, 0) : 0;
  }

  public int Ability { get; }

  public int Energy { get; }

  public static Requirements Free { get; } = new("free");

  public int Health { get; }

  public int Keystone { get; }

  public string Logic { get; }

  public IReadOnlyCollection<string> Other { get; }

  public IReadOnlyCollection<Skill> Skills { get; }

  public override string ToString()
  {
    var sb = new StringBuilder();
    sb.Append(Logic);
    sb.Append(' ');

    if (Health > 0)
    {
      sb.Append($"Health={Health} ");
    }

    if (Energy > 0)
    {
      sb.Append($"Energy={Energy} ");
    }

    if (Ability > 0)
    {
      sb.Append($"Ability={Ability} ");
    }

    if (Keystone > 0)
    {
      sb.Append($"Keystone={Keystone} ");
    }

    sb.Append(string.Join(" ", Skills));
    sb.Append(' ');
    sb.Append(string.Join(" ", Other));

    return sb.ToString().Trim();
  }
}