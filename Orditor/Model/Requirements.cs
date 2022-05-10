using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orditor.Model;

internal class Requirements
{
  public Requirements(string raw)
  {
    var parts = raw.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
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
      else
      {
        if (p[0] == "Health")
        {
          Health += Convert.ToInt32(p[1]);
        }
        else if (p[0] == "Energy")
        {
          Energy += Convert.ToInt32(p[1]);
        }
        else if (p[0] == "Ability")
        {
          Ability += Convert.ToInt32(p[1]);
        }
        else if (p[0] == "Keystone")
        {
          Keystone += Convert.ToInt32(p[1]);
        }
      }
    }

    Skills = skills;
    Other = other;
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