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

public class Building : MyObject
{
    public int HP;
    public int Price;
    public int UpgradeCost;
    public int RepairCost;
    public int buildingCost;

    public Building() : base()
    {

    }

    public Building(int Type, string id, int Level, int HP, int Price, int UpgradeCost, int RepairCost) : base(Type, id, Level)
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

public class OurForces : MyObject
{
    public int HP;
    public int Attack;
    public int SkillCool;
    public int Price;
    public int UpgradeCost;
    public double HealCost;

    public OurForces() { }

    public OurForces(int Type, string id, int Level, int HP, int Attack, int SkillCool, int Price, int UpgradeCost, double HealCost) : base(Type, id, Level)
    {
        this.HP = HP;
        this.Attack = Attack;
        this.SkillCool = SkillCool;
        this.Price = Price;
        this.UpgradeCost = UpgradeCost;
        this.HealCost = HealCost;
    }
}

public class Secret : MyObject
{
    public int Fame;
    public double SecretBanditsGenChance;
    public int Price;

    public Secret() { }

    public Secret(int Type, string id, int Fame, double SecretBanditsGenChance, int Price) : base(Type, id, 1)
    {
        this.Fame = Fame;
        this.SecretBanditsGenChance = SecretBanditsGenChance;
        this.Price = Price;
    }
}

public class Trap : MyObject
{
    public int Attack;
    public int Price;
    public int UpgradeCost;
    public double CoolTime;
    public int DisassemblyTime;
    public Trap() { }

    public Trap(int Type, string id, int Level, int Price, int UpgradeCost, double CoolTime) : base(Type, id, Level)
    {
        this.Price = Price;
        this.UpgradeCost = UpgradeCost;
        this.CoolTime = CoolTime;
    }
}

public class Enemy : MyObject
{
    public int HP;
    public int Fame;
    public int Attack;
    public int BuildingAttack;

    public Enemy() { }

    public Enemy(int Type, string id, int HP, int Attack, int Fame) : base(Type, id, 1)
    {
        this.HP = HP;
        this.Fame = Fame;
        this.Attack = Attack;
    }
}