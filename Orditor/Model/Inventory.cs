using System;
using System.Collections.Generic;

namespace Orditor.Model;

internal class Inventory
{
  public int AbilityCells { get; set; }
  public bool Bash { get; set; }
  public bool CasualCore { get; set; }
  public bool CasualDboost { get; set; }
  public bool ChargeFlame { get; set; }
  public bool ChargeJump { get; set; }
  public bool CleanWater { get; set; }
  public bool Climb { get; set; }
  public bool Dash { get; set; }
  public bool Dbash { get; set; }
  public bool DoubleJump { get; set; }
  public int Energy { get; set; }
  public bool ExpertAbilities { get; set; }
  public bool ExpertCore { get; set; }
  public bool ExpertDboost { get; set; }
  public bool ExpertLure { get; set; }
  public bool ForlornKey { get; set; }
  public bool GinsoKey { get; set; }
  public bool Gjump { get; set; }
  public bool Glide { get; set; }
  public bool Glitched { get; set; }
  public bool Grenade { get; set; }
  public int Health { get; set; }
  public bool HoruKey { get; set; }
  public bool Insane { get; set; }
  public int Keystones { get; set; }
  public int MapFragments { get; set; }
  public bool MasterAbilities { get; set; }
  public bool MasterCore { get; set; }
  public bool MasterDboost { get; set; }
  public bool MasterLure { get; set; }
  public bool OpenDungeons { get; set; }
  public bool OpenWorld { get; set; }
  public bool StandardAbilities { get; set; }
  public bool StandardCore { get; set; }
  public bool StandardDboost { get; set; }
  public bool StandardLure { get; set; }
  public bool Stomp { get; set; }
  public bool TimedLevel { get; set; }
  public bool TpBlackroot { get; set; }
  public bool TpForlorn { get; set; }
  public bool TpGinso { get; set; }
  public bool TpGlades { get; set; }
  public bool TpGrotto { get; set; }
  public bool TpGrove { get; set; }
  public bool TpHoru { get; set; }
  public bool TpHoruFields { get; set; }
  public bool TpLostGrove { get; set; }
  public bool TpSorrow { get; set; }
  public bool TpSwamp { get; set; }
  public bool TpValley { get; set; }
  public bool WallJump { get; set; }
  public bool WindRestored { get; set; }

  public static Inventory Default()
  {
    var inventory = new Inventory();
    inventory.Keystones = 40;
    inventory.AbilityCells = 40;
    inventory.MapFragments = 11;
    inventory.Health = 15;
    inventory.Energy = 15;
    inventory.CasualCore = true;
    inventory.CasualDboost = true;
    inventory.StandardCore = true;
    inventory.StandardDboost = true;
    inventory.StandardAbilities = true;
    inventory.StandardLure = true;
    inventory.OpenDungeons = true;
    inventory.WallJump = true;
    inventory.ChargeFlame = true;
    inventory.DoubleJump = true;
    inventory.Bash = true;
    inventory.Stomp = true;
    inventory.Glide = true;
    inventory.Climb = true;
    inventory.ChargeJump = true;
    inventory.Dash = true;
    inventory.Grenade = true;
    inventory.GinsoKey = true;
    inventory.ForlornKey = true;
    inventory.HoruKey = true;
    inventory.CleanWater = true;
    inventory.WindRestored = true;
    return inventory;
  }

  public bool Fulfills(Requirements requirement)
  {
    foreach (var skill in requirement.Skills)
    {
      if (!_fulfills[skill](this))
      {
        return false;
      }
    }

    if (!HasLogic(requirement))
    {
      return false;
    }

    if (!HasOther(requirement))
    {
      return false;
    }

    return true;
  }

  private readonly Dictionary<Skill, Func<Inventory, bool>> _fulfills = new()
  {
    { Skill.WallJump, i => i.WallJump },
    { Skill.ChargeFlame, i => i.ChargeFlame },
    { Skill.DoubleJump, i => i.DoubleJump },
    { Skill.Bash, i => i.Bash },
    { Skill.Stomp, i => i.Stomp },
    { Skill.Glide, i => i.Glide },
    { Skill.Climb, i => i.Climb },
    { Skill.ChargeJump, i => i.ChargeJump },
    { Skill.Dash, i => i.Dash },
    { Skill.Grenade, i => i.Grenade }
  };

  private bool HasOther(Requirements requirement)
  {
    foreach (var key in requirement.Other)
    {
      switch (key)
      {
        case "TPSorrow":
          return TpSorrow;
        case "TPGrotto":
          return TpGrotto;
        case "TPSwamp":
          return TpSwamp;
        case "TPValley":
          return TpValley;
        case "TPGrove":
          return TpGrove;
        case "TPGinso":
          return TpGinso;
        case "TPForlorn":
          return TpForlorn;
        case "TPHoru":
          return TpHoru;
        case "TPHoruFields":
          return TpHoruFields;
        case "TPGlades":
          return TpGlades;
        case "TPBlackroot":
          return TpBlackroot;
        case "TPLostGrove":
          return TpLostGrove;
        case "GinsoKey":
          return GinsoKey;
        case "Water":
          return CleanWater;
        case "ForlornKey":
          return ForlornKey;
        case "Wind":
          return WindRestored;
        case "HoruKey":
          return HoruKey;
      }
    }

    return true;
  }

  private bool HasLogic(Requirements requirement)
  {
    var logic = requirement.Logic.Replace("-", "").ToLowerInvariant();
    switch (logic)
    {
      case "casualcore":
        return CasualCore;
      case "casualdboost":
        return CasualDboost;
      case "standardcore":
        return StandardCore;
      case "standarddboost":
        return StandardDboost;
      case "standardlure":
        return StandardLure;
      case "standardabilities":
        return StandardAbilities;
      case "expertcore":
        return ExpertCore;
      case "expertdboost":
        return ExpertDboost;
      case "expertlure":
        return ExpertLure;
      case "expertabilities":
        return ExpertAbilities;
      case "dbash":
        return Dbash;
      case "mastercore":
        return MasterCore;
      case "masterdboost":
        return MasterDboost;
      case "masterlure":
        return MasterLure;
      case "masterabilities":
        return MasterAbilities;
      case "gjump":
        return Gjump;
      case "glitched":
        return Glitched;
      case "timedlevel":
        return TimedLevel;
      case "insane":
        return Insane;
    }

    return true;
  }
}