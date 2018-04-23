using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyObject
{
    public int Type;
    public string id;
    public int Level;

    public MyObject()
    {

    }

    public MyObject(int Type, string id, int Level)
    {
        this.Type = Type;
        this.id = id;
        this.Level = Level;
    }
    public virtual int getCost() { return Type; }
}

public class BuildingObject : MyObject
{
    public int HP;
    public int Price;
    public int UpgradeCost;
    public int RepairCost;
    public int buildingCost;
    public int ActiveCost;

    public BuildingObject() : base()
    {

    }

    public BuildingObject(int Type, string id, int Level, int HP, int Price, int UpgradeCost, int RepairCost) : base(Type, id, Level)
    {
        this.HP = HP;
        this.Price = Price;
        this.UpgradeCost = UpgradeCost;
        this.RepairCost = RepairCost;
    }

    public override int getCost()
    {
        return buildingCost;
    }
}

public class OurForcesObject : MyObject
{
    public int HP;
    public int Attack;
    public int SkillCool;
    public int Price;
    public int UpgradeCost;
    public double HealCost;
    public int ActiveCost;
    public double AttackRange;
    public double AttackAngle;
    public int HitConstrain;
    public double EnemyRecognizeRangeHalf;
    public double MoveSpeed;
    public double AttackPeriod;
    public double SkillDuration;
    public double SkillCoefficient_Damage;
    public double SkillCoefficient_Util;
    public double SkillCoefficient_Defence;
    public double SkillRange;

    public OurForcesObject() { }

    public OurForcesObject(int Type, string id, int Level, int HP, int Attack, int SkillCool, int Price, int UpgradeCost, double HealCost) : base(Type, id, Level)
    {
        this.HP = HP;
        this.Attack = Attack;
        this.SkillCool = SkillCool;
        this.Price = Price;
        this.UpgradeCost = UpgradeCost;
        this.HealCost = HealCost;
    }
}

public class SecretObject : MyObject
{
    public int Fame;
    public double SecretBanditsGenChance;
    public int Price;

    public SecretObject() { }

    public SecretObject(int Type, string id, int Fame, double SecretBanditsGenChance, int Price) : base(Type, id, 1)
    {
        this.Fame = Fame;
        this.SecretBanditsGenChance = SecretBanditsGenChance;
        this.Price = Price;
    }
}

public class TrapObject : MyObject
{
    public int Attack;
    public int Price;
    public int UpgradeCost;
    public double CoolTime;
    public double DisassemblyTime;
    public int SellPrice;
    public int ActiveCost;
    public double EffectContinuousTime;

    public TrapObject() { }

    public TrapObject(int Type, string id, int Level, int Price, int UpgradeCost, double CoolTime) : base(Type, id, Level)
    {
        this.Price = Price;
        this.UpgradeCost = UpgradeCost;
        this.CoolTime = CoolTime;
    }
}

public class EnemyObject : MyObject
{
    public int HP;
    public int Fame;
    public int Attack;
    public int BuildingAttack;
    public double AttackRange;
    public double AttackAngle;
    public int HitConstrain;
    public double MoveSpeed;
    public double AttackPeriod;
    public double SkillDuration;
    public double SkillCoefficient_Damage;
    public double SkillCoefficient_Util;
    public double SkillCoefficient_Defence;
    public double SkillRange;

    public EnemyObject() { }

    public EnemyObject(int Type, string id, int HP, int Attack, int Fame) : base(Type, id, 1)
    {
        this.HP = HP;
        this.Fame = Fame;
        this.Attack = Attack;
    }
}